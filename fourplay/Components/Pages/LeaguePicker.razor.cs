using System;
using Blazored.LocalStorage;
using fourplay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace fourplay.Components.Pages;
[Authorize]
public partial class LeaguePicker : ComponentBase
{
    [Inject] private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; } = default!;
    [Inject] ILoginHelper _loginHelper { get; set; } = default!;
    private List<LeagueInfo> _leagues { get; set; } = new();
    private int _selectedValue;
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    private void OnSelectedValuesChanged(int value)
    {
        if (value == _selectedValue)
            return;
        _selectedValue = value;
        _localStorage.SetItemAsync("leagueId", value);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
            if (leagueId > 0)
                _selectedValue = leagueId;
            await InvokeAsync(StateHasChanged);
        }

    }
    protected override async Task OnInitializedAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null)
        {
            var leagueMapping = await db.LeagueUserMapping.Where(x => x.UserId == usrId.Id).ToListAsync();
            _leagues = await db.LeagueInfo.Where(x => leagueMapping.Select(x => x.LeagueId).Contains(x.Id)).ToListAsync();
        }
    }

}
