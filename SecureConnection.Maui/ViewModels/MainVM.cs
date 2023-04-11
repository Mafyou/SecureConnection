using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureConnection.Maui.Services;

namespace SecureConnection.Maui.ViewModels;

public partial class MainVM : ObservableObject
{
    [ObservableProperty]
    private string _status = string.Empty;
    private readonly APIService _api;
    [RelayCommand]
    private async Task onUpdateStatus()
    {
        if (MainThread.IsMainThread)
            myMainThreadCode();
        else
            MainThread.BeginInvokeOnMainThread(myMainThreadCode);
    }
    private async void myMainThreadCode()
    {
        await _api.EncryptedSecureUserAuthentification(new DTO.UserDTO { Name = "Mafyou", Password = "test" });
    }
    public MainVM(APIService api)
    {
        _api = api;
    }
}
