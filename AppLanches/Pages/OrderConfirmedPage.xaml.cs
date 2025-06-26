using System.Threading.Tasks;

namespace AppLanches.Pages;

public partial class OrderConfirmedPage : ContentPage
{
	public OrderConfirmedPage()
	{
		InitializeComponent();
	}

    private async void BtnReturn_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync(); // Voltar para a página anterior
    }
}