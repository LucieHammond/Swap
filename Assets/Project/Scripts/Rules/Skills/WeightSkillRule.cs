using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Components;
using Swap.Components.Models;
using Swap.Interfaces;
using System.Linq;

namespace Swap.Rules.Skills
{
    public class WeightSkillRule : GameRule
    {
        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        private SkillMechanic[] m_WeightSensors;
        private RobotBody[] m_Robots;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_WeightSensors = LevelRule.GetSkillMechanics()
                .Where((mechanic) => mechanic.SkillType == SkillType.DetectWeight).ToArray();
            m_Robots = LevelRule.GetRobotBodies();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (SkillMechanic weightSensor in m_WeightSensors)
            {
                bool wasActivated = weightSensor.IsActivated;
                weightSensor.UpdateActivation(m_Robots);

                if (weightSensor.IsActivated != wasActivated)
                {
                    LogicRule.UpdateStatusSignal(weightSensor.Settings.Signal, weightSensor.IsActivated);
                }
            }
        }
        #endregion
    }
}
