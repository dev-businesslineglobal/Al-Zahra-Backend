using GardeningAPI.Application.Interfaces;
using GardeningAPI.Data;
using GardeningAPI.Model;

namespace gardnerAPIs.Services
{
    public class Configuration : IConfig
    {
        private readonly OdbcClient _db;

        public Configuration(OdbcClient db)
        {
            _db = db;
        }

        public async Task<ConfigurationData?> GetConfiguration()
        {
            return await _db.GetConfiguration();
        }

    }
}
