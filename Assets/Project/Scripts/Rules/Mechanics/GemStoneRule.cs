using GameEngine.Core.Unity.Utilities;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class GemStoneRule : GameRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IControllerRule ControllerRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IInteractionRule InteractionRule;

        private PickupDescriptor m_Descriptor;

        private PlayerSoul m_PlayerSoul;
        private GemStone[] m_GemStones;
        private RobotBody[] m_Robots;

        private bool m_IsRetrieving;
        private bool m_IsReleasing;
        private GemStone m_PickedGem;
        private RobotBody m_GemHolder;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<PickupDescriptor>("GemPickup");

            m_PlayerSoul = LevelRule.GetPlayerSoul();
            m_GemStones = LevelRule.GetGemStones();
            m_Robots = LevelRule.GetRobotBodies();
            m_IsRetrieving = false;
            m_IsReleasing = false;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            CheckStonesFromRobots();
            CheckRobotsFromStones();

            if (m_IsRetrieving)
            {
                bool complete = PerformGemRetrieval();
                if (complete)
                    FinishRetrieval();
            }
            else if (m_IsReleasing)
            {
                bool complete = PerformGemRelease();
                if (complete)
                    FinishRelease();
            }
            else
            {
                GemStone interactingGemStone;
                GetPlayerElement<GemStone> getCurrentStone = (player) => player.CurrentRobotBody.CurrentGemStone;
                bool releasePossible = InteractionRule.CheckPriorityInteraction(getCurrentStone, out interactingGemStone);
                bool retrievePossible = !releasePossible && InteractionRule.FindEligibleInteraction(m_GemStones, out interactingGemStone);

                if (ControllerRule.AskedInteraction() && releasePossible)
                {
                    StartRelease(interactingGemStone);
                }
                else if (ControllerRule.AskedInteraction() && retrievePossible)
                {
                    StartRetrieval(interactingGemStone);
                }
            }
        }
        #endregion

        #region private
        private void CheckStonesFromRobots()
        {
            foreach (RobotBody robot in m_Robots)
            {
                if (robot.CurrentGemStone != null && robot.CurrentGemStone.CurrentRobotOwner != robot)
                {
                    robot.ReleaseStone();
                }
            }
        }

        private void CheckRobotsFromStones()
        {
            foreach (GemStone gemstone in m_GemStones)
            {
                if (gemstone.CurrentRobotOwner != null && gemstone.CurrentRobotOwner.CurrentGemStone != gemstone)
                {
                    gemstone.Free();
                }
            }
        }

        private void StartRetrieval(GemStone gemstone)
        {
            m_IsRetrieving = true;
            m_PickedGem = gemstone;
            m_GemHolder = m_PlayerSoul.CurrentRobotBody;

            m_GemHolder.RetrieveStone(m_PickedGem);
            m_PickedGem.RetrievedWithRobot(m_GemHolder);
        }

        private void FinishRetrieval()
        {
            m_IsRetrieving = false;
            m_PickedGem = null;
            m_GemHolder = null;
        }

        private void StartRelease(GemStone gemstone)
        {
            m_IsReleasing = true;
            m_PickedGem = gemstone;
            m_GemHolder = m_PlayerSoul.CurrentRobotBody;
        }

        private void FinishRelease()
        {
            m_GemHolder.ReleaseStone();
            m_PickedGem.Free();

            m_IsReleasing = false;
            m_PickedGem = null;
            m_GemHolder = null;
        }

        private bool PerformGemRetrieval()
        {
            Vector3 retrievePosition = m_GemHolder.ObjectPoint.position;
            Quaternion retrieveRotation = Quaternion.identity;

            return m_PickedGem.transform.MoveTowards(retrievePosition, retrieveRotation, m_Descriptor.RetrieveSpeed, m_Time.DeltaTime);
        }

        private bool PerformGemRelease()
        {
            Vector3 releasePosition = m_GemHolder.ObjectPoint.position + m_GemHolder.ObjectPoint.rotation * m_Descriptor.ReleaseOffset;
            Quaternion releaseRotation = Quaternion.Euler(m_Descriptor.ReleaseAngle);

            return m_PickedGem.transform.MoveTowards(releasePosition, releaseRotation, m_Descriptor.ReleaseSpeed, m_Time.DeltaTime);
        }
        #endregion
    }
}
