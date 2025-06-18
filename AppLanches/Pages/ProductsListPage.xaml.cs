using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ProductsListPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _categoriaId;
    private bool _loginPageDisplayed = false;

    public ProductsListPage(int categoriaId, string categoryName, 
                            ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _categoriaId = categoriaId;
        Title = categoryName ?? "Products";  
    }

    protected override async void OnAppearing()  //esse metodo é chamado quando a página aparece na tela
    {
        base.OnAppearing();
        await GetProductsList(_categoriaId);
    }

    private async Task<IEnumerable<Product>> GetProductsList(int categoryId)  //esse método busca os produtos de uma categoria específica
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("categoria", categoryId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)  //verifica se o usuário está autenticado
            {
                await DisplayLoginPage(); // se não estiver autenticado, exibe a página de login
                return Enumerable.Empty<Product>();
            }

            if (products is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Não foi possivel obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvProducts.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

   
}