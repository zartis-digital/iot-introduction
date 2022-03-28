// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging.EventGrid;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using Zartis.Webinars.CloudyFarm.Messages.Commands;
using Zartis.Webinars.CloudyFarm.Messages.Telemetry;

namespace Zartis.Webinars.CloudyFarm.Rules
{
    public static class IrrigationRules
    {
        private static readonly int HumidityPercentageThresholdForIrrigation = Convert.ToInt32(Environment.GetEnvironmentVariable("MinimumHumidityPercentageThreshold"));
        private static readonly ServiceClient DeviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHubConnectionString"));

        [FunctionName("IrrigationRules")]
        public static async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            var message = Encoding.UTF8.GetString(eventGridEvent.Data.ToArray());
            var messageBody = JObject.Parse(message)["body"].ToString();

            log.LogInformation($"C# IoT Hub trigger function processed a message: {messageBody}");
            var measurement = JsonConvert.DeserializeObject<HumidityMeasurementEvent>(messageBody);

            var shouldIrrigate = measurement.Humidity < HumidityPercentageThresholdForIrrigation; // Business rule to decide whether irrigate or not.
            log.LogInformation($@"Measured humidity was {measurement.Humidity} % and our minimum humidity percentage is {HumidityPercentageThresholdForIrrigation} %. Therefore, should irrigate is {shouldIrrigate}.");

            if (shouldIrrigate)
            {
                var messageBodyToDevice = JsonConvert.SerializeObject(new IrrigateCommand(1, DateTime.UtcNow));
                var messageToDevice = new Message(Encoding.ASCII.GetBytes(messageBodyToDevice));

                await DeviceClient.SendAsync(measurement.Source, messageToDevice);
            }
        }
    }
}