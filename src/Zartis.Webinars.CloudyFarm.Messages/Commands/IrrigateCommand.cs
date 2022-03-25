using System;

namespace Zartis.Webinars.CloudyFarm.Messages.Commands
{
    /// <summary>
    /// Represents an system's order to initiate an irrigation action.
    /// </summary>
    public class IrrigateCommand
    {
        /// <summary>
        /// Defines the duration in seconds of this irrigation action.
        /// </summary>
        /// <value>
        /// 1
        /// </value>
        public int IrrigationTimeInSeconds { get; }

        /// <summary>
        /// Defines when this irrigation order was issued, in UTC.
        /// </summary>
        public DateTime TimeStampUtc { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrrigateCommand"/> class.
        /// </summary>
        /// <param name="irrigationTimeInSeconds">The irrigation time in seconds.</param>
        /// <param name="timeStampUtc">The time stamp UTC.</param>
        public IrrigateCommand(int irrigationTimeInSeconds, DateTime timeStampUtc)
        {
            TimeStampUtc = timeStampUtc;
            IrrigationTimeInSeconds = irrigationTimeInSeconds;
        }
    }
}