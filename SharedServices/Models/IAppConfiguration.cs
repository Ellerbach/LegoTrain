// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

namespace SharedServices.Models
{
    /// <summary>
    /// Interface for application configuration that can be persisted.
    /// </summary>
    public interface IAppConfiguration
    {
        /// <summary>
        /// Saves the current configuration.
        /// </summary>
        public void Save();
    }
}
