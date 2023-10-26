using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fourplay.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NodaTime;

namespace fourplay.Pages;

public partial class Scores : ComponentBase {
    [Inject]
    private IESPNApiService? _espn { get; set; } = default!;
    private ESPNScores? _scores = null;

    protected override async Task OnInitializedAsync() {
        /*var timer = new Timer(x =>
        {
            InvokeAsync(() =>
            {
                _scores = await _espn.GetScores();
                StateHasChanged();
            }
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(6));
        */
    }

    private DateTime ConvertTimeToCST(DateTime utcDateTime) {
        // Create an instance of the BclDateTimeZone representing the Central Standard Time (CST) zone.
        var cstZone = DateTimeZoneProviders.Tzdb["America/Chicago"];

        // Change the Kind to Local.
        var utc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        // Create an Instant from the UTC DateTime.
        var instant = Instant.FromDateTimeUtc(utc);

        // Convert the instant to the Central Standard Time.
        var zonedDateTime = instant.InZone(cstZone);

        // Return the DateTime in CST.
        return zonedDateTime.ToDateTimeUnspecified();
    }
    private string DisplayDetails(Competition? competition) {
        if (competition.Status.Type.Name == TypeName.StatusFinal) {
            return "FINAL";
        }
        else if (competition.Status.Type.Name == TypeName.StatusScheduled) {
            return competition.Date.ToString("ddd h:mm");
        }
        return null;
    }
    private string BufferTeamName(string abbreviation) => (abbreviation + " ").Substring(0, 3);
}
