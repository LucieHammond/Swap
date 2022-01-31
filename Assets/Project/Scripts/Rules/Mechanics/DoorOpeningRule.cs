using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Components;
using Swap.Interfaces;

namespace Swap.Rules.Mechanics
{
    public class DoorOpeningRule : GameRule
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
                bool open = LogicRule.IsActiveAll(door.SignalsToListen);
                door.Animator.SetBool("Open", open);
            }
        }
        #endregion
    }
}
