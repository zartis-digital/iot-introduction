using System;

namespace Zartis.Webinars.CloudyFarm.DataCollector.Actuators
{
    /// <summary>
    /// An actuator is a component of a machine that is responsible for moving and controlling a mechanism or system, for example by opening a valve.
    /// In simple terms, it is a "mover".
    /// See https://en.wikipedia.org/wiki/Actuator
    /// </summary>
    public interface IActuator
    {
        /// <summary>
        /// Activates this actuator to perform its action.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates this actuator to stop performing its action.
        /// </summary>
        void Deactivate();
    }
}