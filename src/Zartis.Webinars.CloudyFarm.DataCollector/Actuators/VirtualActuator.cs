using System.Diagnostics;
using System.Threading;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Actuators
{
    /// <inheritdoc cref="IActuator"/>
    public sealed class VirtualActuator : IActuator
    {
        /// <inheritdoc />
        public void Activate()
        {
            Debug.WriteLine("Activating actuator...");
            Thread.Sleep(100);
            Debug.WriteLine("Actuator was activated.");
        }

        /// <inheritdoc />
        public void Deactivate()
        {
            Debug.WriteLine("Deactivating actuator...");
            Thread.Sleep(100);
            Debug.WriteLine("Actuator was deactivated.");
        }
    }
}