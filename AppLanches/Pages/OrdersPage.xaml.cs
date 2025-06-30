using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public OrdersPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListOrders();
    }

    private async Task GetListOrders()
    {
        try
        {

            //Exibe o indicador de carregamento
            loadOrdersIndicator.IsRunning = true;
            loadOrdersIndicator.IsVisible = true;

            var (orders, errorMessage) = await _apiService.GetOrdersByUser(Preferences.Get("userId", 0));

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Aviso", "Não existem pedidos para o cliente.", "OK");
                return;
            }
            if (orders is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter pedidos.", "OK");
                return;
            }
            else
            {
                CvOrders.ItemsSource = orders;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os pedidos. Tente novamente mais tarde.", "OK");
        }
        finally
        {
            //Esconde o indicador de carregamento
            loadOrdersIndicator.IsRunning = false;
            loadOrdersIndicator.IsVisible = false;
        }

    }

    private void CvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as OrdersByUser;

        if (selectedItem == null) return;

        Navigation.PushAsync(new OrderDetailsPage(selectedItem.Id,
                                                    selectedItem.Total,
                                                    _apiService,
                                                    _validator));

        ((CollectionView)sender).SelectedItem = null;

    }


    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}