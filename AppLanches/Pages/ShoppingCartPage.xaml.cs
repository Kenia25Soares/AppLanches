using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;

namespace AppLanches.Pages;

public partial class ShoppingCartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator; 
    private bool _loginPageDisplayed = false;


    private ObservableCollection<ShoppingCartItem>
        ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();  //Criando uma instance de ObservableCollection para armazenar
                                                                           //os itens do carrinho de compras

    public ShoppingCartPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetShoppingCartItems();
    }

    private async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems()
    {
        try
        {
            var userId = Preferences.Get("userId", 0);
            var (shoopingCartItems, errorMessage) = await
                     _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirecionar para a pagina de login
                await DisplayLoginPage();
                return Enumerable.Empty<ShoppingCartItem>();
            }

            if (shoopingCartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Não foi possivel obter os itens do carrinho.", "OK");
                return Enumerable.Empty<ShoppingCartItem>();
            }

            ShoppingCartItems.Clear();
            foreach (var item in shoopingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            CvCart.ItemsSource = shoopingCartItems;
            UpdateTotalPrice(); // Atualizar o preco total ap?s atualizar os itens do carrinho

            return shoopingCartItems;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<ShoppingCartItem>();
        }
    }

    private void UpdateTotalPrice()
    {
        try
        {
            var totalPrice = ShoppingCartItems.Sum(item => item.UnitPrice * item.Quantity); 
            LblTotalPrice.Text = totalPrice.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Ocorreu um erro ao atualizar o preço total: {ex.Message}", "OK"); 
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void BtnDecrease_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrease_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {

    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }
}