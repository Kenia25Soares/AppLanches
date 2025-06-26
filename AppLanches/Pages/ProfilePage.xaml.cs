using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public ProfilePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;

        //LblUserName.Text = Preferences.Get("userName", string.Empty);
        LblUserName.Text = Preferences.Get("userName", "Convidado");
        
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnProfile.Source = await GetImageProfile();
    }


    private async Task<string?> GetImageProfile()
    {
        // Obtenha a imagem padr o do AppConfig
        string defaultImage = AppConfig.ProfileImageDefault;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        // Lida com casos de erro
        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "Não foi possivel obter a imagem.", "OK");
                    return defaultImage;
            }
        }

        if (response?.UrlImage is not null)
        {
            return response.ImagePath;
        }

        return defaultImage;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }


    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file is null) return null;

            using (var stream = await file.OpenReadAsync()) 
            using (var memoryStream = new MemoryStream()) 
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Erro", "A funcionalidade não é suportada neste dispositivo.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Erro", "Permissões não concedidas para acessar a câmera ou galeria.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao selecionar a imagem: {ex.Message}", "OK");
        }

        return null;
    }


    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imagemArray = await SelectImageAsync();
            if (imagemArray is null)
            {
                await DisplayAlert("Erro", "Não foi possível carregar a imagem.", "OK");
                return;
            }

            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imagemArray));

            // Envia a imagem para o servidor
            var response = await _apiService.UploadImageUser(imagemArray);
            if (response.Data)
            {
                await DisplayAlert("Sucess", "Imagem enviada com sucesso!", "OK");
            }
            else
            {
                await DisplayAlert("Erro", response.ErrorMessage ?? "Ocorreu um erro desconhecido.", "Cancelar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
        }
    }

    private void TapOrders_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new OrdersPage(_apiService, _validator));
    }

    private void TapMyAccount_Tapped(object sender, TappedEventArgs e)
    {

    }

   

    private void TapQuestions_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }
}