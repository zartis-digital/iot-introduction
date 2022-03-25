using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Actuators
{
    /// <inheritdoc cref="IActuator"/>
    public sealed class WaterPump : IActuator
    {
        private readonly int _outputPin;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaterPump"/> class.
        /// </summary>
        /// <param name="pinForDigitalOutput">The pin for digital output.</param>
        public WaterPump(int pinForDigitalOutput)
        {
            _outputPin = pinForDigitalOutput;
        }

        /// <inheritdoc />
        public void Activate()
        {
            Debug.WriteLine("Activating water pump...");
            SendSignalToWaterPump(PinValue.Low);
            Debug.WriteLine("Water pump was activated.");
        }

        /// <inheritdoc />
        public void Deactivate()
        {
            Debug.WriteLine("Deactivating water pump...");
            SendSignalToWaterPump(PinValue.High);
            Debug.WriteLine("Water pump was deactivated.");
        }

        private void SendSignalToWaterPump(PinValue value)
        {
            using (var controller = new GpioController())
            {
                controller.OpenPin(_outputPin, PinMode.Output);
                controller.Write(_outputPin, value);
                Thread.Sleep(100); // let the system complete the action
            }
        }
    }
}