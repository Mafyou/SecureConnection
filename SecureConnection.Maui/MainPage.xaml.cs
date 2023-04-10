using SecureConnection.Maui.Services;
using SecureConnection.Maui.ViewModels;

namespace SecureConnection.Maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public MainPage(APIService api)
        {
            InitializeComponent();
            BindingContext = new MainVM(api);
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
    }
}