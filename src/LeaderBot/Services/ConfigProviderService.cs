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
        
        public T Config { get; private set; }
        
        public async Task LoadAsync()
        {
            _logger.Debug($"Loading the configuration file {_configName}.");
    
            T config;

            Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
    
            if (!File.Exists(_configPath))
            {
                Config = (T) Activator.CreateInstance(typeof(T));
                    
                await File.WriteAllTextAsync(_configPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
                    
                _logger.Trace($"A new configuration file called {_configName} has been created.");
            }
            else
            {
                Config = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(_configPath));
                    
                _logger.Trace($"The existing configuration file {_configName} has been loaded.");
            }
        }
    }
}