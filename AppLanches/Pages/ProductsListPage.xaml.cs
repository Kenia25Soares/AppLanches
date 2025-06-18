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

    protected override async void OnAppearing()  //esse metodo � chamado quando a p�gina aparece na tela
    {
        base.OnAppearing();
        await GetProductsList(_categoriaId);
    }

    private async Task<IEnumerable<Product>> GetProductsList(int categoryId)  //esse m�todo busca os produtos de uma categoria espec�fica
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("categoria", categoryId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)  //verifica se o usu�rio est� autenticado
            {
                await DisplayLoginPage(); // se n�o estiver autenticado, exibe a p�gina de login
                return Enumerable.Empty<Product>();
            }

            if (products is null)
            {
                await DisplayAlert("Error", errorMessage ?? "N�o foi possivel obter as categorias.", "OK");
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