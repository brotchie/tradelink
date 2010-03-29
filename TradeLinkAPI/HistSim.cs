using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.API
{
    /// <summary>
    /// historical simulation
    /// </summary>
    public interface HistSim
    {
        event TickDelegate GotTick;
        event DebugDelegate GotDebug;

        /// <summary>
        /// reset simulation
        /// </summary>
        void Reset();
        /// <summary>
        /// start simulation and run to specified date/time
        /// </summary>
        /// <param name="ftime"></param>
        void PlayTo(long ftime);
        /// <summary>
        /// stop simulation
        /// </summary>
        void Stop();

        /// <summary>
        /// Total ticks available for processing, based on provided filter or tick files.
        /// </summary>
        int TicksPresent { get; }
        /// <summary>
        /// Ticks processed in this simulation run.
        /// </summary>
        int TicksProcessed { get; }
        /// <summary>
        /// Gets next tick in the simulation
        /// </summary>
        long NextTickTime { get ;  }
    }
}
