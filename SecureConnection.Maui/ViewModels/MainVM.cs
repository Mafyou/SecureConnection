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
        //if (MainThread.IsMainThread)
        //    myMainThreadCode();

        //else
        //    MainThread.BeginInvokeOnMainThread(myMainThreadCode);
        myMainThreadCode();
    }
    private async void myMainThreadCode()
    {
        var sync = await MainThread.GetMainThreadSynchronizationContextAsync();
        var popo = "lol";
        sync.Send(async (e) =>
        {
            Status = await _api.EncryptedSecureUserAuthentification(new DTO.UserDTO { Name = "Mafyou", Password = "test" });
        }, popo);
    }
    public MainVM(APIService api)
    {
        _api = api;
    }
}
