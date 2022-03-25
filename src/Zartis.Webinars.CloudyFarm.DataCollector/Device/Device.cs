using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zartis.Webinars.CloudyFarm.DataCollector.Actuators;
using Zartis.Webinars.CloudyFarm.DataCollector.Events;
using Zartis.Webinars.CloudyFarm.DataCollector.Metrics;
using Zartis.Webinars.CloudyFarm.DataCollector.Sensors;
using Zartis.Webinars.CloudyFarm.Messages.Commands;
using Zartis.Webinars.CloudyFarm.Messages.Telemetry;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Device
{
    /// <inheritdoc cref="IDevice"/>
    public sealed class Device : IDevice
    {
        private const string IotHubConnectionString = "Your Azure IoT Hub connection string";
        private const short MeasurementIntervalInSeconds = 10;

        private readonly DeviceClient _deviceClient;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ISensor _sensor;
        private readonly IActuator _actuator;
        private readonly IReporter _reporter;

        private bool _turnedOn;

        /// <summary>
        /// Gets the identifier for this device.
        /// </summary>
        /// <value>
        /// device-1
        /// </value>
        public string Id { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="sensor">The sensor.</param>
        /// <param name="actuator">The actuator.</param>
        /// <param name="reporter">The reporter.</param>
        public Device(string id, ISensor sensor, IActuator actuator, IReporter reporter)
        {
            Id = id;
            _sensor = sensor;
            _actuator = actuator;
            _reporter = reporter;
            _deviceClient = DeviceClient.CreateFromConnectionString(IotHubConnectionString, TransportType.Mqtt);

            _cancellationTokenSource = new CancellationTokenSource();

            sensor.NewMeasurementAvailable += OnNewMeasurementAvailable;
        }

        private void OnNewMeasurementAvailable(object sender, SensorMeasurementEvent sensorMeasurementEvent)
        {
            Task.Factory.StartNew(async () => await SendMeasurementToIoTHub(Id, sensorMeasurementEvent, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        private async Task SendMeasurementToIoTHub(string source, SensorMeasurementEvent sensorMeasurementEvent, CancellationToken cancellationTokenForReceivingMessagesTask)
        {
            try
            {
                cancellationTokenForReceivingMessagesTask.ThrowIfCancellationRequested();

                var humidityEvent = new HumidityMeasurementEvent((double)sensorMeasurementEvent.Measurement, DateTime.FromBinary(sensorMeasurementEvent.MeasurementUtcDate), source);
                _reporter.ReportHumidityMeasurement(humidityEvent.Humidity);

                var message = BuildMessageToSend(humidityEvent);
                await _deviceClient.SendEventAsync(message, cancellationTokenForReceivingMessagesTask);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("SendMeasurementToIoTHub task was cancelled.");
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"Error sending event to IoT Hub: {exc}");
            }
        }

        private static Message BuildMessageToSend(HumidityMeasurementEvent humidityEvent)
        {
            var messageContent = JsonConvert.SerializeObject(humidityEvent);

            Debug.WriteLine($"Message content to send: {messageContent}");
            return new Message(Encoding.ASCII.GetBytes(messageContent))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };
        }

        /// <inheritdoc />
        public void TurnOn()
        {
            if (!_turnedOn)
            {
                _sensor.StartAutoMeasurement(MeasurementIntervalInSeconds * 1000);
                Task.Factory.StartNew(async () => await ReceiveCloudMessagesAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

                _turnedOn = true;
                Debug.WriteLine("Device was turned on.");
            }
        }

        private async Task ReceiveCloudMessagesAsync(CancellationToken cancellationTokenForReceivingMessagesTask)
        {
            var continueReceiving = true;
            while (continueReceiving) // Receive while task is not cancelled
            {
                try
                {
                    cancellationTokenForReceivingMessagesTask.ThrowIfCancellationRequested();

                    Message receivedMessage = null;
                    try
                    {
                        receivedMessage = await _deviceClient.ReceiveAsync(cancellationTokenForReceivingMessagesTask);
                        if (receivedMessage == null) continue;

                        await _deviceClient.CompleteAsync(receivedMessage, cancellationTokenForReceivingMessagesTask);

                        using (var reader = new StreamReader(receivedMessage.BodyStream))
                        {
                            var messageBody = await reader.ReadToEndAsync();
                            Debug.WriteLine($"Received message from IoT Hub: {messageBody}");

                            var irrigationCommand = JsonConvert.DeserializeObject<IrrigateCommand>(messageBody);
                            _reporter.ReportIrrigationAction(irrigationCommand.IrrigationTimeInSeconds);

                            _actuator.Activate();
                            Thread.Sleep(irrigationCommand.IrrigationTimeInSeconds * 1000);
                            _actuator.Deactivate();
                        }

                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine($"Error receiving event form the IoT Hub: {exc}");

                        if (receivedMessage != null)
                            await _deviceClient.RejectAsync(receivedMessage, cancellationTokenForReceivingMessagesTask);
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("ReceiveCloudMessagesAsync task was cancelled.");
                    continueReceiving = false;
                }
            }
        }

        /// <inheritdoc />
        public void TurnOff()
        {
            if (_turnedOn)
            {
                _sensor.StopAutoMeasurement();
                _cancellationTokenSource.Cancel();

                _turnedOn = false;
                Debug.WriteLine("Device was turned off.");
            }
        }
    }
}