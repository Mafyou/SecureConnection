namespace SecureConnection.Maui.ViewModels;

public partial class MainVM : ObservableObject
{
    [ObservableProperty]
    private string _status = string.Empty;
    private readonly APIService _api;
    [RelayCommand]
    private async Task onUpdateStatus()
    {
        var result = await TaskGoup.RunScopeAsync(default, async group =>
        {
            return await TaskGoup.RaceScopeAsync<UserDTO>(group.CancellationToken, async group =>
            {
                group.Race(async token => await _api.EncryptedSecureUserUnCryptedRaced(new UserDTO { Name = "Mafyou1", Password = "test1" }));
                group.Race(async token => await _api.EncryptedSecureUserUnCryptedRaced(new UserDTO { Name = "Mafyou2", Password = "test2" }));
                group.Race(async token => await _api.EncryptedSecureUserUnCryptedRaced(new UserDTO { Name = "Mafyou3", Password = "test3" }));
            });
        });
        Status = $"{result.Name} {result.Password}";
    }
    public MainVM(APIService api)
    {
        _api = api;
    }
}
