namespace in3D.AvatarsSDK.Configurations
{
    public class ServerQrConfigurator : ServerConfigurator
    {
        public void ConfigFromQr(string qrText)
        {
            string[] parts = qrText.Split('-');
            
            RewriteConfiguration($"{{\"vendor\":\"{parts[1]}\"}}");
        }
    }
}