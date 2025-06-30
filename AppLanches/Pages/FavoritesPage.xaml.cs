using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class FavoritesPage : ContentPage
{

    private readonly FavoritesService _favoritesService;
    private readonly ApiService _apiService; 
    private readonly IValidator _validator;

    public FavoritesPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _favoritesService = ServiceFactory.CreateFavoritesService();
        _apiService = apiService;
        _validator = validator;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteProducts();
    }


    private async Task GetFavoriteProducts()
    {

        try
        {
            var favoriteProducts = await _favoritesService.ReadAllAsync();

            if (favoriteProducts is null || favoriteProducts.Count == 0)
            {
                CvProducts.ItemsSource = null;
                LblWarning.IsVisible = true; 
            }
            else
            {
                CvProducts.ItemsSource = favoriteProducts;
                LblWarning.IsVisible = false; 
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
        }
    }


    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId,
                                                     currentSelection.Name!, 
                                                     _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}