namespace in3D.AvatarsSDK.Configurations
{
    public class UserFromQrConfigurator : UserConfigurator
    {
        public void AuthorizeWithQr(string qrText)
        {
            string[] parts = qrText.Split('-');

            configuration.ConnectWithQr(parts[0]);
            ReadConfiguration();
        }
    }
}
