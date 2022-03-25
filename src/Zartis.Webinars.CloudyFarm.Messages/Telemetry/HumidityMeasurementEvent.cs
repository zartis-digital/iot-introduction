using System;

namespace Zartis.Webinars.CloudyFarm.Messages.Telemetry
{
    /// <summary>
    /// Telemetry event containing information about a humidity measurement.
    /// </summary>
    public class HumidityMeasurementEvent
    {
        /// <summary>
        /// Gets the date and time when this event was created, in UTC.
        /// </summary>
        public DateTime TimeStampUtc { get; }

        /// <summary>
        /// Gets the humidity percentage associated to this measurement event.
        /// </summary>
        /// <value>
        /// 10.5
        /// </value>
        public double Humidity { get; }

        /// <summary>
        /// Gets the source of this measurement event.
        /// </summary>
        /// <value>
        /// device-1
        /// </value>
        public string Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HumidityMeasurementEvent"/> class.
        /// </summary>
        /// <param name="humidity">The humidity.</param>
        /// <param name="measurementTimeStampUtc">The measurement time stamp UTC.</param>
        /// <param name="source">The source.</param>
        public HumidityMeasurementEvent(double humidity, DateTime measurementTimeStampUtc, string source = "Unknown")
        {
            Humidity = humidity;
            TimeStampUtc = measurementTimeStampUtc;
            Source = source;
        }
    }
}