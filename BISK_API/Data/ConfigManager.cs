namespace GardeningAPI.Data
{
    public sealed class ConfigManager
    {
        private static readonly object padlock = new object();

        private static ConfigManager? instance;

        private IConfigurationRoot? _config;

        ConfigManager()
        {
        }

        public static ConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new ConfigManager();
                        }
                    }
                }
                return instance;
            }
        }
        public void init()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
        public IConfigurationRoot getConfig()
        {
            if (_config == null)
            {
                init();
            }
            return _config!;
        }
    }
}