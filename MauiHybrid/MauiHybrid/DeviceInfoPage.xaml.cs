namespace MauiHybrid
{
    public partial class DeviceInfoPage : ContentPage
    {
        public DeviceInfoPage()
        {
            InitializeComponent();
            ReadDeviceInfo();
        }

        private void ReadDeviceInfo()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine($"📱 Modelo: {DeviceInfo.Current.Model}");
            sb.AppendLine($"🏭 Fabricante: {DeviceInfo.Current.Manufacturer}");
            sb.AppendLine($"📝 Nombre: {DeviceInfo.Current.Name}");
            sb.AppendLine($"🖥️ Versión del SO: {DeviceInfo.Current.VersionString}");
            sb.AppendLine($"📊 Idiom: {DeviceInfo.Current.Idiom}");
            sb.AppendLine($"💻 Plataforma: {DeviceInfo.Current.Platform}");

            bool isVirtual = DeviceInfo.Current.DeviceType switch
            {
                DeviceType.Physical => false,
                DeviceType.Virtual => true,
                _ => false
            };

            sb.AppendLine($"🔧 ¿Dispositivo Virtual? {(isVirtual ? "Sí" : "No")}");

            // Información adicional
            sb.AppendLine($"\n📏 Densidad de Pantalla: {DeviceDisplay.Current.MainDisplayInfo.Density}");
            sb.AppendLine($"📐 Resolución: {DeviceDisplay.Current.MainDisplayInfo.Width} x {DeviceDisplay.Current.MainDisplayInfo.Height}");
            sb.AppendLine($"🔄 Orientación: {DeviceDisplay.Current.MainDisplayInfo.Orientation}");

            DisplayDeviceLabel.Text = sb.ToString();
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            ReadDeviceInfo();
            DisplayAlert("✅ Actualizado", "La información del dispositivo se ha actualizado correctamente.", "OK");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

