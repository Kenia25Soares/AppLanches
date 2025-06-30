using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public OrderDetailsPage(int orderId,
                              decimal priceTotal, 
                              ApiService apiService, 
                              IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        LblPriceTotal.Text = "�" + priceTotal;

        GetOrderDetail(orderId);
    }

    private async void GetOrderDetail(int orderId)
    {
        try
        {
            //Exibe o indicador de carregamento
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;

            var (orderDetails, errorMessage) = await _apiService.GetOrderDetails(orderId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (orderDetails is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "N�o foi poss�vel obter detalhes do pedido.", "OK");
                return;
            }
            else
            {
                CvOrderDetails.ItemsSource = orderDetails;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os detalhes. Tente novamente mais tarde.", "OK");
        }
        finally
        {
            //Esconde o indicador de carregamento
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}