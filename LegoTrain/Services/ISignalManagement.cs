// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models;

namespace LegoTrain.Services
{
    public interface ISignalManagement
    {
        void ChangeSignal(byte NumSignal, SignalState value);
        void Dispose();
        SignalState GetSignal(byte NumSignal);
    }
}