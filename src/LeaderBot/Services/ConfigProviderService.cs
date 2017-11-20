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

        private readonly JsonSerializerSettings _settings;
        
        public ConfigProviderService(ILogger logger, JsonSerializerSettings settings)
        {
            var rootDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var configDir = Path.Combine(rootDir, "Config");
            
            _logger = logger;
            _configName = $"{typeof(T).Name.ToPascalCase()}.json";
            _configPath = Path.Combine(configDir, _configName);
            _settings = settings;
        }
        
        public string Checksum { get; private set; }
        
        public T Config { get; private set; }
        
        public async Task LoadAsync()
        {
            _logger.Debug($"Loading the configuration file {_configName}.");

            Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
    
            if (!File.Exists(_configPath))
            {
                Config = (T) Activator.CreateInstance(typeof(T));
                    
                _logger.Trace($"A new configuration file called {_configName} has been created.");
            }
            else
            {
                var data = await File.ReadAllTextAsync(_configPath);
                
                Config = JsonConvert.DeserializeObject<T>(data, _settings);
                Checksum = data.GetChecksum();
                    
                _logger.Trace($"The existing configuration file {_configName} has been loaded.");
            }

            // Save current config to a file. Will also add any new config values. 
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            _logger.Trace($"Trying to save the configuration file {_configName}.");

            var data = Config.ToJson(Formatting.Indented, _settings);
            var dataChecksum = data.GetChecksum();

            if (!string.IsNullOrEmpty(Checksum) && Checksum == dataChecksum)
            {
                return;
            }
            
            _logger.Debug($"Saving the configuration file {_configName} because there was a checksum change.");

            await File.WriteAllTextAsync(_configPath, data);

            Checksum = dataChecksum;
        }
    }
}