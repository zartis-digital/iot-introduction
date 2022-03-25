namespace Zartis.Webinars.CloudyFarm.DataCollector.Metrics
{
    /// <summary>
    /// Represents the component that reports humidity measurements and irrigation actions to third party systems
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Reports the irrigation action.
        /// </summary>
        /// <param name="irrigationPeriodInSeconds">The irrigation period in seconds.</param>
        void ReportIrrigationAction(int irrigationPeriodInSeconds);

        /// <summary>
        /// Reports a humidity measurement.
        /// </summary>
        /// <param name="humidityMeasurement">The humidity percentage measurement.</param>
        void ReportHumidityMeasurement(double humidityMeasurement);
    }
}