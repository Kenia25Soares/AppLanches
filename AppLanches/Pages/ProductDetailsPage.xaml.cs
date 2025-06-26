using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;
    private readonly FavoritesService _favoritesService = new FavoritesService();  // Serviço para gerenciar favoritos, Como não foi injetado, cria uma nova instância aqui
    private string? _urlImage;

    public ProductDetailsPage(int productId, string productName,
                               ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _productId = productId;
        Title = productName ?? "Detalhe do Produto";

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
        UpdateFavoriteButton();
    }

    private async Task <Product?> GetProductDetails(int productId)
    {
        var (productDetails, errorMessage) = await _apiService.GetProductDetails(productId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        // Verificar se houve algum erro na obtenção das produtos
        if (productDetails is null)
        {
            // Lidar com o erro, exibir mensagem ou logar
            await DisplayAlert("Error", errorMessage ?? "Não foi possível obter o produto.", "OK");
            return null;
        }

        if (productDetails != null)
        {
            // Atualiza as propriedades dos controles com os dados do produto
            ProductImage.Source = productDetails.ImagePath;
            LblProductName.Text = productDetails.Name;
            LblProductPrice.Text = productDetails.Price.ToString();
            LblProductDescription.Text = productDetails.Details;
            LblTotalPrice.Text = productDetails.Price.ToString();
            _urlImage = productDetails.ImagePath;
        }
        else
        {
            await DisplayAlert("Error", errorMessage ?? "Não foi possível obter os detalhes do produto.", "OK");
            return null;
        }
        return productDetails;
    }

    private async void ImageBtnFavorite_Clicked(object sender, EventArgs e)
    {

        try
        {
            var favoriteExists = await _favoritesService.ReadAsync(_productId);
            if (favoriteExists is not null)
            {
                await _favoritesService.DeleteAsync(favoriteExists);
            }
            else
            {
                var favoriteProduct = new FavoriteProduct()
                {
                    ProductId = _productId,
                    IsFavorite = true,
                    Details = LblProductDescription.Text,
                    Name = LblProductName.Text,
                    Price = Convert.ToDecimal(LblProductPrice.Text),
                    UrlImage = _urlImage  // Definir _urlImage com o caminho da imagem do produto
                };

                await _favoritesService.CreateAsync(favoriteProduct); 
            }
            UpdateFavoriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private  async void UpdateFavoriteButton()
    {
        var favoriteExists = await
               _favoritesService.ReadAsync(_productId);

        if (favoriteExists is not null)
            ImageBtnFavorite.Source = "heartfill";
        else
            ImageBtnFavorite.Source = "heart";
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
          decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Decrementa a quantidade, e não permite que seja menor que 1
            quantity = Math.Max(1, quantity - 1);
            LblQuantity.Text = quantity.ToString();

            // Calcula o preço total
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            // Tratar caso as conversões falhem
            DisplayAlert("Error", "Valores inválidos", "OK");
        }
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
         decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Incrementa a quantidade
            quantity++;
            LblQuantity.Text = quantity.ToString();

            // Calcula o preço total
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString(); // Formata como moeda
        }
        else
        {
            // Tratar caso as conversões falhem
            DisplayAlert("Error", "Valores inválidos", "OK");
        }
    }

    private async void BtnAddToCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            var shoppingCart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                Total = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("userId", 0)
            };
            var response = await _apiService.AddItemToCart(shoppingCart);
            if (response.Data)
            {
                await DisplayAlert("Success", "Item adicionado ao carrinho!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Falha ao adicionar item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}
