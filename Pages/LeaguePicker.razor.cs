using fourplay.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace fourplay.Pages;

public partial class LeaguePicker : ComponentBase {
    [Inject]
    private ApplicationDbContext? _db { get; set; } = default!;
    [Inject]
    ILoginHelper _loginHelper { get; set; }
    private List<LeagueInfo> _leagues { get; set; } = new();
    MudListItem<string> _selectedItem;
    object _selectedValue;
    [Inject] Blazored.LocalStorage.ILocalStorageService _localStorage { get; set; }
    public object SelectedValue {
        get => _selectedValue;
        set {
            if (value == _selectedValue)
                return;

            _selectedValue = value;
            _localStorage.SetItemAsync("leagueId", (int)value);
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
            if (leagueId > 0)
                _selectedValue = (object)leagueId;
            await InvokeAsync(StateHasChanged);
        }
    }
    protected override async Task OnInitializedAsync() {
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null) {
            var leagueMapping = _db.LeagueUserMapping.Where(x => x.UserId == usrId.Id);
            _leagues = _db.LeagueInfo.Where(x => leagueMapping.Select(x => x.LeagueId).Contains(x.Id)).ToList();
        }
    }

}
