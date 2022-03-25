using System;
using Zartis.Webinars.CloudyFarm.DataCollector.Events;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Sensors
{
    /// <summary>
    /// A sensor is a device that produces an output signal for the purpose of sensing of a physical phenomenon.
    /// See https://en.wikipedia.org/wiki/Sensor
    /// </summary>
    public interface ISensor : IDisposable
    {
        /// <summary>
        /// Occurs periodically once <see cref="StartAutoMeasurement"/> is invoked and based on its interval setting.
        /// </summary>
        event EventHandler<SensorMeasurementEvent> NewMeasurementAvailable;

        /// <summary>
        /// Starts the automatic and periodic measurement.
        /// </summary>
        /// <param name="measurementIntervalInMilliseconds">The measurement interval in milliseconds.</param>
        void StartAutoMeasurement(int measurementIntervalInMilliseconds);

        /// <summary>
        /// Stops the automatic and periodic measurement.
        /// </summary>
        void StopAutoMeasurement();
    }

}