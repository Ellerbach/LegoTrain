// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;

namespace SharedServices.Models
{
    public class ConfigurationEventArgs : EventArgs
    {
        public ConfigurationEventArgs(string paramName)
        {
            ParamName = paramName;
        }

        public string ParamName { get; set; }
    }
}
