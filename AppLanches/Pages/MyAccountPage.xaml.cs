using AppLanches.Services;

namespace AppLanches.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string UserNameKey = "username";
    private const string EmailUserKey = "useremail";
    private const string PhoneUserKey = "userphone";

    public MyAccountPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInformation();
        ImgBtnProfile.Source = await GetImageProfileAsync();
    }

    private void LoadUserInformation()
    {


        LblUserName.Text = Preferences.Get(UserNameKey, string.Empty);
        EntName.Text = LblUserName.Text;

        EntEmail.Text = Preferences.Get(EmailUserKey, string.Empty);
        EntPhone.Text = Preferences.Get(PhoneUserKey, string.Empty);
    }

    private async Task<string?> GetImageProfileAsync()
    {
        string defaultImage = AppConfig.ProfileImageDefault; 

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Erro", "Não autorizado", "OK");
                    return defaultImage;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter a imagem.", "OK");
                    return defaultImage;
            }
        }

        if (!string.IsNullOrEmpty(response?.UrlImage))
        {
            return $"{AppConfig.BaseUrl.TrimEnd('/')}{response.UrlImage}";
        }
        return defaultImage;
    }


    private async void BtnSave_Clicked(object sender, EventArgs e)
    {
        // Salva as informa  es alteradas pelo usuário nas preferências
        Preferences.Set(UserNameKey, EntName.Text);
        Preferences.Set(EmailUserKey, EntEmail.Text);
        Preferences.Set(PhoneUserKey, EntPhone.Text);
        await DisplayAlert("Informações Salvas", "Suas informações foram salvas com sucesso!", "OK");
    }
}