using Iot.Device.Ads1115;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;
using Zartis.Webinars.CloudyFarm.DataCollector.Events;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Sensors
{
    /// <inheritdoc cref="ISensor"/>
    public sealed class HumiditySensor : ISensor
    {
        public event EventHandler<SensorMeasurementEvent> NewMeasurementAvailable;

        private readonly Timer _watchDog;
        private int _measurementIntervalInMilliseconds;
        private readonly Ads1115 _ads1115;

        /// <summary>
        /// Initializes a new instance of the <see cref="HumiditySensor"/> class.
        /// </summary>
        public HumiditySensor()
        {
            _watchDog = new Timer(MeasurementCallback, null, Timeout.Infinite, Timeout.Infinite);
            _measurementIntervalInMilliseconds = 1000;

            var settings = new I2cConnectionSettings(1, (int)I2cAddress.GND);
            var device = I2cDevice.Create(settings);
            _ads1115 = new Ads1115(device);
        }

        private void MeasurementCallback(object state)
        {
            _watchDog.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (NewMeasurementAvailable is null)
                    Debug.WriteLine($"{nameof(NewMeasurementAvailable)} event was not assigned to any handler.");

                else
                {
                    NewMeasurementAvailable(this, new SensorMeasurementEvent(GetHumidityPercentage()));
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"Problem with periodic measuring {exc}");
            }
            finally
            {
                _watchDog.Change(_measurementIntervalInMilliseconds, Timeout.Infinite);
            }
        }

        private double GetHumidityPercentage()
        {
            const short waterValue = 6800; // Represents the value of our sensor when it is inside a cup of water, 100% of humidity. Probably too simplistic... but it is an starting point
            var measurement = _ads1115.ReadRaw();

            return Math.Round((waterValue * 100) / (double)measurement, 2);
        }

        /// <inheritdoc/>
        public void StartAutoMeasurement(int measurementIntervalInMilliseconds)
        {
            _measurementIntervalInMilliseconds = measurementIntervalInMilliseconds;
            _watchDog.Change(_measurementIntervalInMilliseconds, Timeout.Infinite);
        }

        /// <inheritdoc/>
        public void StopAutoMeasurement()
        {
            _watchDog.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _watchDog?.Dispose();
        }
    }
}