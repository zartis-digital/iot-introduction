namespace Zartis.Webinars.CloudyFarm.DataCollector.Device
{
    /// <summary>
    /// Represents an IoT Data Collector device
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Turns on this device.
        /// </summary>
        void TurnOn();

        /// <summary>
        /// Turns off this device.
        /// </summary>
        void TurnOff();
    }
}