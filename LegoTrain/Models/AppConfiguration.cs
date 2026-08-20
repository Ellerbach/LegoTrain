// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Lego.Infrared;
using LegoTrain.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LegoTrain.Models
{
    /// <summary>
    /// Represents the main application configuration for the Lego train system.
    /// Manages all trains, signals, switches, detectors, and device discovery.
    /// </summary>
    public class AppConfiguration
    {
        private static readonly string ConfigName = $".{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}config.json";
        private LegoDiscovery _disco;

        /// <summary>
        /// Loads the application configuration from the config file.
        /// If the file doesn't exist or can't be loaded, returns a new default configuration.
        /// </summary>
        /// <returns>The loaded or default application configuration.</returns>
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
            // No update as no Discovery at this point
            // config.UpdateConfiguration();
            return config;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        public AppConfiguration()
        {
        }

        /// <summary>
        /// Saves the current application configuration to the config file.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the device discovery service.
        /// When set, automatically updates the configuration with discovered devices.
        /// </summary>
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

        /// <summary>
        /// Updates the configuration by initializing all service executors with the current discovery service.
        /// </summary>
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

            try
            {
                DetectorManagement = new DetectorManagement(Discovery, this);
            }
            catch
            {
                // Nothing
            }
        }

        /// <summary>
        /// Gets or sets the Lego infrared command executor.
        /// </summary>
        [JsonIgnore]
        public LegoInfraredExecutor? LegoInfraredExecutor { get; internal set; }

        /// <summary>
        /// Gets or sets the switch management service.
        /// </summary>
        [JsonIgnore]
        public ISwitchManagement? SwitchManagement { get; internal set; }

        /// <summary>
        /// Gets or sets the signal management service.
        /// </summary>
        [JsonIgnore]
        public ISignalManagement? SignalManagement { get; internal set; }

        /// <summary>
        /// Gets or sets the detector management service.
        /// </summary>
        [JsonIgnore]
        public IDetectorManagement? DetectorManagement { get; internal set; }

        /// <summary>
        /// Gets or sets the list of configured trains.
        /// </summary>
        public List<Train> Trains { get; set; } = new List<Train>();

        /// <summary>
        /// Gets or sets the list of configured signals.
        /// </summary>
        public List<Signal> Signals { get; set; } = new List<Signal>();

        /// <summary>
        /// Gets or sets the list of configured switches.
        /// </summary>
        public List<Switch> Switches { get; set; } = new List<Switch>();

        /// <summary>
        /// Gets or sets the list of configured detectors.
        /// </summary>
        public List<Detector> Detectors { get; set; } = new List<Detector>();

        /// <summary>
        /// Gets or sets the infrared device configuration.
        /// </summary>
        [JsonIgnore]
        public Infrared Infrared { get; internal set; } = new Infrared();
    }
}
