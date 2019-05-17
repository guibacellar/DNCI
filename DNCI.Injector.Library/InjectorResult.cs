using System;

namespace DNCI.Injector.Library
{
    /// <summary>
    /// Injector Result Object
    /// </summary>
    public sealed class InjectorResult
    {
        /// <summary>
        /// Target Process Name
        /// </summary>
        public String TargetProcessName { get; set; }

        /// <summary>
        /// Target Process ID
        /// </summary>
        public Int32 TargetProcessId { get; set; }

        /// <summary>
        /// Processing Status
        /// </summary>
        public InjectorResultStatus Status { get; set; } = InjectorResultStatus.NOT_PROCESSED;

        public override string ToString()
        {
            return String.Format("PID={0}, Name={1}, Status={2}", this.TargetProcessId, this.TargetProcessName, this.Status);
        }
    }
}
