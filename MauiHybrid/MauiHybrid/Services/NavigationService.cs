namespace MauiHybrid.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToDeviceInfoAsync()
        {
            // Obtener la navegación desde la MainPage
            var mainPage = Application.Current?.MainPage;
            
            if (mainPage?.Navigation != null)
            {
                await mainPage.Navigation.PushAsync(new DeviceInfoPage());
            }
        }
    }
}

