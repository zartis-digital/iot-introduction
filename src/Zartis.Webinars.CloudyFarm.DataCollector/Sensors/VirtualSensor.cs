using System;
using System.Diagnostics;
using System.Threading;
using Zartis.Webinars.CloudyFarm.DataCollector.Events;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Sensors
{
    /// <inheritdoc cref="ISensor"/>
    public sealed class VirtualSensor : ISensor
    {
        public event EventHandler<SensorMeasurementEvent> NewMeasurementAvailable;

        private readonly Random _random;
        private readonly Timer _watchDog;
        private int _measurementIntervalInMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualSensor"/> class.
        /// </summary>
        public VirtualSensor()
        {
            _random = new Random();
            _watchDog = new Timer(MeasurementCallback, null, Timeout.Infinite, Timeout.Infinite);
            _measurementIntervalInMilliseconds = 1000;
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
                    NewMeasurementAvailable(this, new SensorMeasurementEvent(_random.Next(0, 100)));
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