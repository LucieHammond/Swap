using Swap.Data.Models;
using System.Collections.Generic;

namespace Swap.Interfaces
{
    public interface ILogicRule
    {
        // Sending signal
        void TriggerInstantSignal(Signal signalId);

        void UpdateStatusSignal(Signal signalId, bool status);

        void AchievePermanentSignal(Signal signalId, bool positive = true);

        // Listening for signals
        bool IsActive(Signal signalId);

        bool IsActiveAll(IEnumerable<Signal> signalIds);

        bool IsActiveAny(IEnumerable<Signal> signalIds);
    }
}

