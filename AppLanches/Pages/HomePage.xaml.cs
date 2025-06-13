using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService; 
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        LblUserName.Text = "Olá, " + Preferences.Get("userName", string.Empty);
        _apiService = apiService;
        _validator = validator;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCategoriesList();
        await GetBestSellers();
        await GetPopular();
    }


    private async Task<IEnumerable<Category>> GetCategoriesList()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategories.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }


    private async Task<IEnumerable<Product>> GetBestSellers()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("maisvendido", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvBestSellers.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocorreu um erro inesperado: {ex.Message}", "OK"); 
            return Enumerable.Empty<Product>();
        }
    }


    private async Task<IEnumerable<Product>> GetPopular()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>(); 
            }

            if (products == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }
            CvPopular.ItemsSource = products;
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

    private void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void CvBestSellers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void CvPopular_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}