// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System;

namespace nanoDiscovery.Common
{
    [Flags]
    public enum DeviceCapability
    {
        None = 0b0000_0000,
        Signal = 0b0000_0001,
        Switch = 0b0000_0010,
        Infrared = 0b0000_0100,
    }
}
