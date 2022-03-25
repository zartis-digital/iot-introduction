using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Zartis.Webinars.CloudyFarm.DataCollector.Actuators;
using Zartis.Webinars.CloudyFarm.DataCollector.Device;
using Zartis.Webinars.CloudyFarm.DataCollector.Metrics;
using Zartis.Webinars.CloudyFarm.DataCollector.Sensors;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Zartis.Webinars.CloudyFarm.DataCollector
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const string DeviceId = "device-1";
        private const int WaterPumpDigitalOutputPin = 18;

        private BackgroundTaskDeferral _taskDeferral;
        private IDevice _dataCollectorDevice;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;

            try
            {
                var sensor = new HumiditySensor(); // or VirtualSensor;
                var actuator = new WaterPump(WaterPumpDigitalOutputPin); // or VirtualActuator
                var reporter = new InfluxReporter();

                _dataCollectorDevice = new Device.Device(DeviceId, sensor, actuator, reporter);
                _dataCollectorDevice.TurnOn();
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"Error running the background task. {exc}");
                _dataCollectorDevice?.TurnOff();
                _taskDeferral.Complete();
            }
            finally
            {
                // TODO :: GetDeferral & extendedBackgroundTaskTime not working as expected to keep background task running behind the scenes.
                //         Dirty way to keep main thread up and running.
                Thread.Sleep(60 * 60 * 1000); // 1 hour
            }
        }

        private void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine($"Background task was cancelled. Reason: {reason}");

            _dataCollectorDevice?.TurnOff();
            _taskDeferral?.Complete();
        }
    }
}
