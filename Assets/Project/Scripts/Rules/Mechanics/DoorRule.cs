using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Components;
using Swap.Interfaces;

namespace Swap.Rules.Mechanics
{
    public class DoorRule : GameRule
    {
        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        private Door[] m_Doors;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Doors = LevelRule.GetDoors();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (Door door in m_Doors)
            {
                bool signalValue = LogicRule.IsActiveAll(door.SignalsToListen);

                if (signalValue && !door.IsOpen)
                    door.Open();

                else if (!signalValue && door.IsOpen)
                    door.Close();
            }
        }
        #endregion
    }
}
