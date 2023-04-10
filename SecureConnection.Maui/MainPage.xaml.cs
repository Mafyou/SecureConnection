using SecureConnection.Maui.Services;

namespace SecureConnection.Maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly APIService _apiService;
        public MainPage(APIService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void ButtonAuthentification_Clicked(object sender, EventArgs e)
        {
            lResult.Text = await _apiService.SecureUserAuthentification(new DTO.UserDTO { Name = "Mafyou", Password = "test" });
        }
    }
}