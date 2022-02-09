using GameEngine.Core.Unity.Utilities;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Linq;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class GemPickupRule : GameRule
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
        private Transform m_GemsRoot;

        private bool m_IsRetrievingGem;
        private bool m_IsReleasingGem;
        private GemStone m_PickedGemStone;
        private RobotBody m_RobotHoldingGem;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<PickupDescriptor>("GemPickup");

            m_PlayerSoul = LevelRule.GetPlayerSoul();
            m_GemStones = LevelRule.GetGemStones();
            m_GemsRoot = LevelRule.GetRootTransform("Objects");
            m_IsRetrievingGem = false;
            m_IsReleasingGem = false;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (m_IsRetrievingGem)
            {
                bool complete = PerformGemRetrieval();
                if (complete)
                {
                    m_IsRetrievingGem = false;
                    m_PickedGemStone = null;
                    m_RobotHoldingGem = null;
                }
            }
            else if (m_IsReleasingGem)
            {
                bool complete = PerformGemRelease();
                if (complete)
                {
                    ReleaseControlOfGem(m_PickedGemStone, m_RobotHoldingGem);
                    
                    m_IsReleasingGem = false;
                    m_PickedGemStone = null;
                    m_RobotHoldingGem = null;
                }
            }
            else
            {
                GemStone interactingGemStone;
                bool releasePossible = InteractionRule.CheckPriorityInteraction(GetCurrentGemStone, out interactingGemStone);
                bool retrievePossible = !releasePossible && InteractionRule.FindEligibleInteraction(m_GemStones, out interactingGemStone);

                if (ControllerRule.AskedInteraction() && releasePossible)
                {
                    m_IsReleasingGem = true;
                    m_PickedGemStone = interactingGemStone;
                    m_RobotHoldingGem = m_PlayerSoul.CurrentRobotBody;
                }
                else if (ControllerRule.AskedInteraction() && retrievePossible)
                {
                    m_IsRetrievingGem = true;
                    m_PickedGemStone = interactingGemStone;
                    m_RobotHoldingGem = m_PlayerSoul.CurrentRobotBody;

                    TakeControlOfGem(m_PickedGemStone, m_RobotHoldingGem);
                }
            }
        }
        #endregion

        #region private
        private GemStone GetCurrentGemStone(PlayerSoul currentState)
        {
            if (currentState.CurrentlyHeldGems.TryGetValue(currentState.CurrentRobotBody, out GemStone gemStone))
                return gemStone;

            return null;
        }

        private void TakeControlOfGem(GemStone gemStone, RobotBody robotHoldingGem)
        {
            RobotBody currentOwner = m_PlayerSoul.CurrentlyHeldGems.FirstOrDefault(p => p.Value == gemStone).Key;
            if (currentOwner != null) m_PlayerSoul.CurrentlyHeldGems[currentOwner] = null;
            
            m_PlayerSoul.CurrentlyHeldGems[robotHoldingGem] = gemStone;
            gemStone.transform.SetParent(robotHoldingGem.ObjectRoot, true);
            gemStone.RigidBody.isKinematic = true;
            gemStone.Collider.isTrigger = true;
        }

        private void ReleaseControlOfGem(GemStone gemStone, RobotBody robotHoldingGem)
        {
            m_PlayerSoul.CurrentlyHeldGems[robotHoldingGem] = null;
            gemStone.transform.SetParent(m_GemsRoot, true);
            gemStone.RigidBody.isKinematic = false;
            gemStone.Collider.isTrigger = false;
        }

        private bool PerformGemRetrieval()
        {
            Vector3 retrievePosition = m_RobotHoldingGem.ObjectRoot.position;
            Quaternion retrieveRotation = Quaternion.identity;

            return m_PickedGemStone.transform.MoveTowards(retrievePosition, retrieveRotation, m_Descriptor.RetrieveSpeed, m_Time.DeltaTime);
        }

        private bool PerformGemRelease()
        {
            Vector3 releasePosition = m_RobotHoldingGem.ObjectRoot.position + m_RobotHoldingGem.ObjectRoot.rotation * m_Descriptor.ReleaseOffset;
            Quaternion releaseRotation = Quaternion.Euler(m_Descriptor.ReleaseAngle);

            return m_PickedGemStone.transform.MoveTowards(releasePosition, releaseRotation, m_Descriptor.ReleaseSpeed, m_Time.DeltaTime);
        }
        #endregion
    }
}
