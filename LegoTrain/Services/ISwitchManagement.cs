// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

namespace LegoTrain.Services
{
    public interface ISwitchManagement
    {
        void ChangeSwitch(byte NumSignal, bool value);
        void Dispose();
        bool GetSwitch(byte NumSignal);
    }
}