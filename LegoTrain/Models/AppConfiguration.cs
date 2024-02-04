// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Lego.Infrared;
using LegoTrain.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LegoTrain.Models
{
    public class AppConfiguration
    {
        private static readonly string ConfigName = $".{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}config.json";
        private LegoDiscovery _disco;

        public static AppConfiguration Load()
        {
            AppConfiguration? config = null;
            try
            {
                var str = File.ReadAllText(ConfigName);
                config = JsonSerializer.Deserialize<AppConfiguration>(str);
            }
            catch
            {
                // We swallow it, most likely not configured
            }

            config = config ?? new AppConfiguration();
            config.UpdateConfiguration();
            return config;
        }

        public AppConfiguration()
        {
        }

        public void Save()
        {
            try
            {
                var str = JsonSerializer.Serialize(this);
                File.WriteAllText(ConfigName, str);
            }
            catch
            {
                // We swallow it, most likely not configured
            }
        }

        [JsonIgnore]
        public LegoDiscovery Discovery
        {
            get => _disco;
            set
            {
                _disco = value;
                UpdateConfiguration();
            }
        }

        public void UpdateConfiguration()
        {
            try
            {
                LegoInfraredExecutor = new LegoInfraredExecutor(Discovery);
            }
            catch
            {
                // Nothing
            }

            try
            {
                SignalManagement = new SignalManagement(Discovery);
                //#endif
            }
            catch
            {
                // Nothing
            }

            try
            {
                SwitchManagement = new SwitchManagement(Discovery);
            }
            catch
            {
                // Nothing
            }
        }

        [JsonIgnore]
        public LegoInfraredExecutor? LegoInfraredExecutor { get; internal set; }

        [JsonIgnore]
        public ISwitchManagement? SwitchManagement { get; internal set; }

        [JsonIgnore]
        public ISignalManagement? SignalManagement { get; internal set; }

        public List<Train> Trains { get; set; } = new List<Train>();

        public List<Signal> Signals { get; set; } = new List<Signal>();

        public List<Switch> Switches { get; set; } = new List<Switch>();

        [JsonIgnore]
        public Infrared Infrared { get; internal set; } = new Infrared();
    }
}
