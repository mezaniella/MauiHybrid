namespace MauiHybrid.Services
{
    /// <summary>
    /// Helper class to manage HttpClient configuration and Url endpoint addresses.
    /// </summary>
    internal class HttpClientHelper
    {
        //TODO: Place this in AppSettings or Client config file
        private static string _baseUrl = "https://localhost:44363/";
        public static string BaseUrl
        {
            get
            {
#if ANDROID || IOS

                return _baseUrl.Replace( "localhost","192.168.2.164");
#endif

                return _baseUrl;
            }
        }
        public static string LoginUrl => $"{BaseUrl}identity/login";
        public static string RefreshUrl => $"{BaseUrl}identity/refresh";
        public static string WeatherUrl => $"{BaseUrl}api/weather";

        public static HttpClient GetHttpClient()
        {
#if WINDOWS || MACCATALYST
            return new HttpClient();
#else
            return new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler());
#endif
        }
    }

    internal class HttpsClientHandlerService
    {
        public HttpMessageHandler GetPlatformMessageHandler()
        {
#if ANDROID
            var handler = new Xamarin.Android.Net.AndroidMessageHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
#elif IOS
            var handler = new NSUrlSessionHandler
            {
                TrustOverrideForUrl = IsHttpsLocalhost
            };
            return handler;
#else
            throw new PlatformNotSupportedException("Only Android and iOS supported.");
#endif
        }

#if IOS
        public bool IsHttpsLocalhost(NSUrlSessionHandler sender, string url, Security.SecTrust trust)
        {
            return true;
        }
#endif
    }

}
