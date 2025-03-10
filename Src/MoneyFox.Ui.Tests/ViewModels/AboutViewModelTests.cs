namespace MoneyFox.Ui.Tests.ViewModels;

using System.Diagnostics.CodeAnalysis;
using Core.Common.Interfaces;
using Core.Interfaces;
using FluentAssertions;
using NSubstitute;
using Views.About;
using Xunit;

[ExcludeFromCodeCoverage]
public class AboutViewModelTests
{
    [Fact]
    public void Version_NoParams_ReturnCorrectMail()
    {
        var appinfos = Substitute.For<IAppInformation>();
        _ = appinfos.GetVersion.Returns("42");
        new AboutViewModel(
                appInformation: appinfos,
                emailAdapter: Substitute.For<IEmailAdapter>(),
                browserAdapter: Substitute.For<IBrowserAdapter>(),
                storeOperations: Substitute.For<IStoreOperations>()).Version.Should()
            .Be("42");
    }

    [Fact]
    public async Task GoToWebsite_NoParams_Called()
    {
        var webBrowserTaskSetup = Substitute.For<IBrowserAdapter>();
        _ = webBrowserTaskSetup.OpenWebsiteAsync(Arg.Any<Uri>()).Returns(Task.CompletedTask);
        new AboutViewModel(
            appInformation: Substitute.For<IAppInformation>(),
            emailAdapter: Substitute.For<IEmailAdapter>(),
            browserAdapter: webBrowserTaskSetup,

            // ReSharper disable once MethodHasAsyncOverload
            storeOperations: Substitute.For<IStoreOperations>()).GoToWebsiteCommand.Execute(null);

        await webBrowserTaskSetup.Received(1).OpenWebsiteAsync(Arg.Is<Uri>(s => s == new Uri("https://www.apply-solutions.ch")));
    }

    [Fact]
    public async Task GoToRepository_NoParams_CommandCalled()
    {
        var webbrowserTaskSetup = Substitute.For<IBrowserAdapter>();
        _ = webbrowserTaskSetup.OpenWebsiteAsync(Arg.Any<Uri>()).Returns(Task.CompletedTask);
        new AboutViewModel(
            appInformation: Substitute.For<IAppInformation>(),
            emailAdapter: Substitute.For<IEmailAdapter>(),
            browserAdapter: webbrowserTaskSetup,
            storeOperations: Substitute.For<IStoreOperations>()).GoToRepositoryCommand.Execute(null);

        await webbrowserTaskSetup.Received(1).OpenWebsiteAsync(Arg.Is<Uri>(s => s == new Uri("https://github.com/MoneyFox/MoneyFox")));
    }

    [Fact]
    public void RateApp_NoParams_CommandCalled()
    {
        var storeFeaturesSetup = Substitute.For<IStoreOperations>();
        new AboutViewModel(
            appInformation: Substitute.For<IAppInformation>(),
            emailAdapter: Substitute.For<IEmailAdapter>(),
            browserAdapter: Substitute.For<IBrowserAdapter>(),
            storeOperations: storeFeaturesSetup).RateAppCommand.Execute(null);

        storeFeaturesSetup.Received(1).RateApp();
    }
}
