# Aplicación Híbrida .NET MAUI Blazor con Autenticación

Este ejemplo demuestra cómo construir aplicaciones híbridas .NET MAUI Blazor y aplicaciones web que comparten UI común y proporcionan *autenticación*. Utiliza ASP.NET Core Identity con cuentas locales, pero puedes usar este patrón con cualquier proveedor de autenticación.

## 🎥 Video Demo

[![Video Demo](https://img.youtube.com/vi/EnVpeOiR1tc/maxresdefault.jpg)](https://www.youtube.com/watch?v=EnVpeOiR1tc)

*Haz clic en la imagen para ver el video de demostración*

## Características Principales

- ✅ Autenticación compartida entre la app MAUI y la aplicación web
- ✅ UI compartida usando componentes Razor Blazor
- ✅ Navegación híbrida entre páginas Blazor y páginas XAML nativas de MAUI
- ✅ Acceso a funcionalidades nativas del dispositivo desde páginas XAML
- ✅ Almacenamiento seguro de tokens en el dispositivo
- ✅ Llamadas a endpoints protegidos desde el cliente

## Ejecutar el Proyecto

1. Clona el repositorio.
2. Asegúrate de tener [.NET 10 instalado con la carga de trabajo MAUI](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-9.0&tabs=vswin).
3. Abre la solución en Visual Studio 2022.
4. Establece el proyecto `MauiHybrid` como proyecto de inicio.
5. Inicia el proyecto `MauiHybrid.Web` sin depuración (clic derecho → "Depurar" → "Iniciar sin depurar").
6. Registra un usuario en la aplicación web Blazor o navega a `https://localhost:7157/swagger` para usar el endpoint `/identity/register`.
7. Inicia (F5) el proyecto `MauiHybrid`. Puedes ejecutarlo en Windows o un emulador Android.
8. Inicia sesión con el usuario que registraste.
9. Explora las páginas compartidas (Counter, Weather) y la página nativa de especificaciones del dispositivo.

## Navegación Híbrida: Blazor ↔ XAML Nativo

Una de las características más importantes de esta aplicación es la capacidad de navegar entre páginas Blazor y páginas XAML nativas de MAUI. Esto demuestra el verdadero poder de las aplicaciones híbridas.

### Página Nativa de Especificaciones del Dispositivo

La aplicación incluye una página XAML nativa (`DeviceInfoPage.xaml`) que muestra información del dispositivo físico:

- Modelo y fabricante
- Versión del sistema operativo
- Resolución y densidad de pantalla
- Si es un dispositivo físico o virtual

Esta página se puede acceder desde el menú de navegación Blazor, demostrando la integración perfecta entre ambos mundos.

### Cómo Funciona la Navegación Híbrida

1. **Desde Blazor a XAML Nativo**: El menú Blazor contiene un enlace que navega a una página Razor (`DeviceInfo.razor`), que a su vez utiliza un servicio de navegación (`INavigationService`) para abrir la página XAML nativa.

2. **Desde XAML Nativo a Blazor**: La página nativa tiene un botón de retroceso que vuelve a la aplicación Blazor usando la navegación estándar de MAUI.

```csharp
// Servicio de navegación que permite ir de Blazor a XAML
public class NavigationService : INavigationService
{
    public async Task NavigateToDeviceInfoAsync()
    {
        var mainPage = Application.Current?.MainPage;
        if (mainPage?.Navigation != null)
        {
            await mainPage.Navigation.PushAsync(new DeviceInfoPage());
        }
    }
}
```

### Configuración de la Navegación

El proyecto está configurado para usar `NavigationPage` como contenedor principal, lo que permite la navegación nativa:

```csharp
// App.xaml.cs
protected override Window CreateWindow(IActivationState? activationState)
{
    return new Window(new NavigationPage(new MainPage())) 
    { 
        Title = "MauiHybrid" 
    };
}
```

La página principal de Blazor (`MainPage.xaml`) tiene la barra de navegación oculta para mantener la experiencia full-screen:

```xml
<ContentPage NavigationPage.HasNavigationBar="False"
             NavigationPage.HasBackButton="False">
    <BlazorWebView ... />
</ContentPage>
```

## Componentes Compartidos

La UI compartida está en el proyecto `MauiHybrid.Shared`. Este proyecto contiene los componentes Razor que se comparten entre la aplicación MAUI y la aplicación web Blazor (páginas Home, Counter y Weather).

Las páginas `Counter.razor` y `Weather.razor` están protegidas con el atributo `[Authorize]`, por lo que no puedes navegar a ellas a menos que estés autenticado.

```razor
@page "/counter"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize]
```

## Autenticación

### Configuración del Servidor

La aplicación web Blazor expone los endpoints de ASP.NET Identity para que puedan ser llamados por clientes externos (como la app MAUI). Esto se configura en `Program.cs`:

```csharp
// Necesario para que clientes externos puedan autenticarse
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options => 
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ...

// Mapear los endpoints de identidad
app.MapGroup("/identity").MapIdentityApi<ApplicationUser>();
```

### Autenticación desde el Cliente MAUI

El `MauiAuthenticationStateProvider` gestiona el estado de autenticación del usuario. Utiliza `HttpClient` para hacer peticiones al servidor y almacena los tokens de forma segura usando `SecureStorage`.

```csharp
// Login desde el cliente MAUI
private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginRequest loginModel)
{
    var httpClient = HttpClientHelper.GetHttpClient();
    var response = await httpClient.PostAsJsonAsync(HttpClientHelper.LoginUrl, loginData);
    
    if (response.IsSuccessStatusCode)
    {
        var token = await response.Content.ReadAsStringAsync();
        await TokenStorage.SaveTokenToSecureStorageAsync(token, loginModel.Email);
        // ...
    }
}
```

### Registro en MauiProgram.cs

El proveedor de autenticación se registra en el contenedor de inyección de dependencias:

```csharp
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<MauiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(s =>
    (MauiAuthenticationStateProvider)s.GetRequiredService<MauiAuthenticationStateProvider>());
```

## Estructura del Proyecto

- **MauiHybrid**: Proyecto MAUI con Blazor WebView y páginas XAML nativas
- **MauiHybrid.Shared**: Componentes Razor compartidos entre MAUI y Web
- **MauiHybrid.Web**: Aplicación web Blazor con autenticación

## Recursos Adicionales

- [Documentación oficial de Blazor Hybrid](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/?view=aspnetcore-10.0)
- [Autenticación en Blazor Hybrid](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/security/?view=aspnetcore-10.0)
- [SecureStorage en MAUI](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage?view=net-maui-10.0)
