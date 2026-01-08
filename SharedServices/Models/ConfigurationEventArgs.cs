// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;

namespace SharedServices.Models
{
    /// <summary>
    /// Provides data for configuration update events.
    /// </summary>
    public class ConfigurationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationEventArgs"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that was updated.</param>
        public ConfigurationEventArgs(string paramName)
        {
            ParamName = paramName;
        }

        /// <summary>
        /// Gets or sets the name of the parameter that was updated.
        /// </summary>
        public string ParamName { get; set; }
    }
}
