namespace LegoTrain.Models.Device
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
