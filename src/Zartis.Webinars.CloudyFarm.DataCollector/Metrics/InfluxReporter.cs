using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Metrics
{
    /// <inheritdoc cref="IReporter"/>
    public sealed class InfluxReporter : IReporter
    {
        const string InfluxRemoteEndpoint = "Your Influx cloud endpoint";
        const string Token = "Your Influx cloud access token";
        const string Organization = "Your Influx cloud Organization name";
        const string Bucket = "Your Influx cloud Bucket name";

        /// <inheritdoc />
        public void ReportIrrigationAction(int irrigationPeriodInSeconds)
        {
            using (var client = InfluxDBClientFactory.Create(InfluxRemoteEndpoint, Token))
            using (var writeApi = client.GetWriteApi())
            {
                var data = PointData
                    .Measurement("cloudyfarm-orders")
                    .Tag("cloudyfarm", "irrigation")
                    .Field("irrigation_order", irrigationPeriodInSeconds)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                writeApi.WritePoint(data, Bucket, Organization);
            }
        }

        /// <inheritdoc />
        public void ReportHumidityMeasurement(double humidityMeasurement)
        {
            using (var client = InfluxDBClientFactory.Create(InfluxRemoteEndpoint, Token))
            using (var writeApi = client.GetWriteApi())
            {
                var data = PointData
                    .Measurement("cloudyfarm-measurements")
                    .Tag("cloudyfarm", "humidity")
                    .Field("humidity_measurement", humidityMeasurement)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                writeApi.WritePoint(data, Bucket, Organization);
            }
        }
    }
}