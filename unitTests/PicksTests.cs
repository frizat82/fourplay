using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using fourplay.Components.Pages;
using fourplay.Services;
using fourplay.Models;
using MudBlazor;
/*
public class PicksTests : TestContext {
    private readonly IFixture _fixture;
    private readonly IESPNApiService _espnApiServiceMock;
    private readonly ILeaderboardService _leaderboardServiceMock;

    public PicksTests() {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _espnApiServiceMock = _fixture.Freeze<IESPNApiService>();
        _leaderboardServiceMock = _fixture.Freeze<ILeaderboardService>();

        Services.AddSingleton(_espnApiServiceMock);
        Services.AddSingleton(_leaderboardServiceMock);
    }

    [Fact]
    public void Component_Should_Initialize_Correctly() {
        // Arrange
        var component = RenderComponent<Picks>();

        // Act
        var mudText = component.FindComponent<MudText>();

        // Assert
        Assert.NotNull(mudText);
    }

    [Fact]
    public async Task Should_Display_Loading_Indicator_When_Loading() {
        // Arrange
        var component = RenderComponent<Picks>();
        var picksInstance = component.Instance;
        picksInstance.GetType().GetProperty("_loading").SetValue(picksInstance, true);

        // Act
        await picksInstance.InvokeAsync(() => picksInstance.StateHasChanged());

        // Assert
        var progressLinear = component.FindComponent<MudProgressLinear>();
        Assert.NotNull(progressLinear);
    }

    [Fact]
    public async Task Should_Display_Scores_When_Scores_Are_Not_Null_And_LeagueId_Is_Greater_Than_0() {
        // Arrange
        var scores = _fixture.Create<List<Score>>();
        _espnApiServiceMock.GetScores().Returns(Task.FromResult(scores));

        var component = RenderComponent<Picks>();
        var picksInstance = component.Instance;
        picksInstance.GetType().GetProperty("_scores").SetValue(picksInstance, scores);
        picksInstance.GetType().GetProperty("_leagueId").SetValue(picksInstance, 1);

        // Act
        await picksInstance.InvokeAsync(() => picksInstance.StateHasChanged());

        // Assert
        var mudText = component.FindComponent<MudText>();
        Assert.NotNull(mudText);
    }

    [Fact]
    public async Task Should_Enable_Submit_Picks_Button_When_Not_Disabled() {
        // Arrange
        var component = RenderComponent<Picks>();
        var picksInstance = component.Instance;
        picksInstance.GetType().GetProperty("_leagueId").SetValue(picksInstance, 1);

        // Act
        await picksInstance.InvokeAsync(() => picksInstance.StateHasChanged());

        // Assert
        var submitButton = component.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Submit Picks"));
        Assert.NotNull(submitButton);
        Assert.False(submitButton.HasAttribute("disabled"));
    }

    [Fact]
    public async Task Should_Clear_Picks_When_Clear_Picks_Button_Is_Clicked() {
        // Arrange
        var component = RenderComponent<Picks>();
        var picksInstance = component.Instance;
        picksInstance.GetType().GetProperty("_leagueId").SetValue(picksInstance, 1);

        // Act
        await picksInstance.InvokeAsync(() => picksInstance.ClearPicks());

        // Assert
        var clearButton = component.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Clear Picks"));
        Assert.NotNull(clearButton);
    }

    [Fact]
    public async Task Should_Select_And_Unselect_Picks_Correctly() {
        // Arrange
        var component = RenderComponent<Picks>();
        var picksInstance = component.Instance;
        picksInstance.GetType().GetProperty("_leagueId").SetValue(picksInstance, 1);

        // Act
        await picksInstance.InvokeAsync(() => picksInstance.SelectPick("TeamA"));
        await picksInstance.InvokeAsync(() => picksInstance.UnSelectPick("TeamA"));

        // Assert
        var pickButton = component.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Pick"));
        Assert.NotNull(pickButton);
    }
}
*/