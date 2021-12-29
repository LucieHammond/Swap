using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Collections.Generic;

namespace Swap.Rules.World
{
    [RuleAccess(typeof(ILogicRule))]
    public class LogicRule : GameRule, ILogicRule
    {
        private Dictionary<Signal, bool> m_SignalValues;
        private HashSet<Signal> m_InstantSignals;
        private HashSet<Signal> m_StatusSignals;
        private HashSet<Signal> m_PermanentSignals;

        public LogicRule()
        {
            m_SignalValues = new Dictionary<Signal, bool>();
            m_InstantSignals = new HashSet<Signal>();
            m_StatusSignals = new HashSet<Signal>();
            m_PermanentSignals = new HashSet<Signal>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            MarkInitialized();
        }

        protected override void Unload()
        {
            m_InstantSignals.Clear();
            m_StatusSignals.Clear();
            m_PermanentSignals.Clear();

            MarkUnloaded();
        }

        protected override void Update() { }

        protected override void LateUpdate()
        {
            foreach (Signal signalId in m_InstantSignals)
            {
                m_SignalValues[signalId] = false;
            }

            m_InstantSignals.Clear();
        }
        #endregion

        #region ILogicRule API
        public void TriggerInstantSignal(Signal signalId)
        {
            if (m_PermanentSignals.Contains(signalId))
                return;
            if (m_StatusSignals.Contains(signalId))
                return;
            if (m_InstantSignals.Contains(signalId))
                return;

            m_SignalValues[signalId] = true;
            
            m_InstantSignals.Add(signalId);
        }

        public void UpdateStatusSignal(Signal signalId, bool status)
        {
            if (m_PermanentSignals.Contains(signalId))
                return;
            if (m_StatusSignals.Contains(signalId) == status)
                return;

            m_SignalValues[signalId] = status;
            
            if (status) m_StatusSignals.Add(signalId); else m_StatusSignals.Remove(signalId);
            m_InstantSignals.Remove(signalId);
        }

        public void AchievePermanentSignal(Signal signalId, bool positive = true)
        {
            if (m_PermanentSignals.Contains(signalId))
                return;

            m_SignalValues[signalId] = positive;
            
            m_PermanentSignals.Add(signalId);
            m_StatusSignals.Remove(signalId);
            m_InstantSignals.Remove(signalId);
        }

        public bool IsActive(Signal signalId)
        {
            return m_SignalValues.ContainsKey(signalId) && m_SignalValues[signalId];
        }

        public bool IsActiveAll(IEnumerable<Signal> signalIds)
        {
            bool active = true;

            foreach (Signal signalId in signalIds)
                active = active && IsActive(signalId);

            return active;
        }

        public bool IsActiveAny(IEnumerable<Signal> signalIds)
        {
            bool active = false;

            foreach (Signal signalId in signalIds)
                active = active || IsActive(signalId);

            return active;
        }
        #endregion
    }
}
