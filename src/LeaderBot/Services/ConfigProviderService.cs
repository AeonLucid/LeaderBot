using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using LeaderBot.Extensions;
using Newtonsoft.Json;

namespace LeaderBot.Services
{
    public class ConfigProviderService<T>
    {
        private readonly ILogger _logger;

        private readonly string _configName;

        private readonly string _configPath;

        public ConfigProviderService(ILogger logger)
        {
            var rootDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var configDir = Path.Combine(rootDir, "Config");
            
            _logger = logger;
            _configName = $"{typeof(T).Name.ToPascalCase()}.json";
            _configPath = Path.Combine(configDir, _configName);
        }
        
        public string Checksum { get; private set; }
        
        public T Config { get; private set; }
        
        public async Task LoadAsync()
        {
            _logger.Debug($"Loading the configuration file {_configName}.");
    
            T config;

            Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
    
            if (!File.Exists(_configPath))
            {
                Config = (T) Activator.CreateInstance(typeof(T));
                    
                _logger.Trace($"A new configuration file called {_configName} has been created.");
            }
            else
            {
                var data = await File.ReadAllTextAsync(_configPath);
                
                Config = JsonConvert.DeserializeObject<T>(data);
                Checksum = data.GetChecksum();
                    
                _logger.Trace($"The existing configuration file {_configName} has been loaded.");
            }

            // Save current config to a file. Will also add any new config values. 
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            _logger.Trace($"Saving the configuration to {_configPath}.");

            var data = Config.ToJson();
            var dataChecksum = data.GetChecksum();

            if (!string.IsNullOrEmpty(Checksum) && Checksum == dataChecksum)
            {
                return;
            }
            
            _logger.Debug($"Saving the configuration to {_configPath} because there was a checksum change.");

            await File.WriteAllTextAsync(_configPath, JsonConvert.SerializeObject(Config, Formatting.Indented));

            Checksum = dataChecksum;
        }
    }
}