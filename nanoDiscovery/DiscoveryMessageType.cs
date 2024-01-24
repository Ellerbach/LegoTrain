// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

namespace nanoDiscovery
{
    public enum DiscoveryMessageType: byte
    {
        None = 0,
        Discovery = 1,
        Capabilities = 2,
        Byebye = 3,
        Message = 4,
    }
}
