namespace in3D.AvatarsSDK.Configurations
{
    /// <summary>
    /// User Configurator
    /// </summary>
    public class UserConfigurator : Alteracia.Patterns.ScriptableObjects.ConfigurableController<UserConfigurator, UserConfiguration>
    {
        protected override void OnConfigurationRead()
        {
            //Users.AddRecord(this.configuration.UserId, this.configuration, true);
        }
    }
}
