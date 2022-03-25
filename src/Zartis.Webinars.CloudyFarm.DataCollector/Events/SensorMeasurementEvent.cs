using System;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Events
{
    /// <summary>
    /// Represents an event generated after sensor measuring a physical phenomenon.
    /// </summary>
    public sealed class SensorMeasurementEvent
    {
        /// <summary>
        /// Gets the measurement UTC date in binary format. It indicates when this measurement event happened.
        /// </summary>
        public long MeasurementUtcDate { get; }

        /// <summary>
        /// Gets the sensor measurement information associated with this event.
        /// </summary>
        public object Measurement { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorMeasurementEvent"/> class.
        /// </summary>
        /// <param name="measurement">The measurement.</param>
        public SensorMeasurementEvent(object measurement)
        {
            MeasurementUtcDate = DateTime.Now.ToUniversalTime().ToBinary();
            Measurement = measurement;
        }
    }
}