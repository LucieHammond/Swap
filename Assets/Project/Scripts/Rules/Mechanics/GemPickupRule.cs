using GameEngine.Core.Unity.Utilities;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Data.Models;
using Swap.Interfaces;
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

        private PickupDescriptor m_Descriptor;

        private LevelState m_LevelState;
        private GemStone[] m_GemStones;
        private Transform m_GemsRoot;

        private bool m_IsRetrievingGem;
        private bool m_IsReleasingGem;
        private GemStone m_PickedGemStone;
        private Transform m_GemHoldingRoot;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<PickupDescriptor>("GemPickup");

            m_LevelState = LevelRule.GetLevelState();
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
                    m_GemHoldingRoot = null;
                }
            }
            else if (m_IsReleasingGem)
            {
                bool complete = PerformGemRelease();
                if (complete)
                {
                    ReleaseControlOfGem(m_PickedGemStone);
                    
                    m_IsReleasingGem = false;
                    m_PickedGemStone = null;
                    m_GemHoldingRoot = null;
                }
            }
            else if (ControllerRule.AskedInteraction() && m_LevelState.CurrentRobotBody != null)
            {
                Transform gemRootOnBody = m_LevelState.CurrentRobotBody.ObjectRoot;

                if (IsHoldingGemStone(gemRootOnBody, out GemStone currentGemStone))
                {
                    m_IsReleasingGem = true;
                    m_PickedGemStone = currentGemStone;
                    m_GemHoldingRoot = gemRootOnBody;
                }
                else if (GetEligibleGemStone(gemRootOnBody, out GemStone eligibleGemStone))
                {
                    m_IsRetrievingGem = true;
                    m_PickedGemStone = eligibleGemStone;
                    m_GemHoldingRoot = gemRootOnBody;

                    TakeControlOfGem(m_PickedGemStone, m_GemHoldingRoot);
                }

                if (m_IsRetrievingGem || m_IsReleasingGem)
                    ControllerRule.ResetInteraction();
            }
        }
        #endregion

        #region private
        private bool IsHoldingGemStone(Transform holdingRoot, out GemStone gemStone)
        {
            gemStone = null;

            if (holdingRoot.childCount > 0)
            {
                gemStone = holdingRoot.GetComponentInChildren<GemStone>();
                return gemStone != null;
            }

            return false;
        }

        private bool GetEligibleGemStone(Transform holdingRoot, out GemStone gemStone)
        {
            gemStone = null;
            float minDistance = float.MaxValue;
            
            Vector3 checkCenter = holdingRoot.position + holdingRoot.rotation * m_Descriptor.CheckOffset;
            for (int i = 0; i < m_GemStones.Length; i++)
            {
                Vector3 gemPosition = m_GemStones[i].transform.position;

                if (Mathf.Abs(gemPosition.y - checkCenter.y) > m_Descriptor.CheckHeight)
                    continue;

                float distanceToCenter = Vector3.Distance(gemPosition, new Vector3(checkCenter.x, gemPosition.y, checkCenter.z));
                if (distanceToCenter > m_Descriptor.CheckRadius)
                    continue;

                if (distanceToCenter < minDistance)
                {
                    minDistance = distanceToCenter;
                    gemStone = m_GemStones[i];
                }
            }

            return gemStone != null;
        }

        private void TakeControlOfGem(GemStone gemStone, Transform gemHoldingRoot)
        {
            gemStone.transform.SetParent(gemHoldingRoot, true);
            gemStone.RigidBody.isKinematic = true;
        }

        private void ReleaseControlOfGem(GemStone gemStone)
        {
            gemStone.transform.SetParent(m_GemsRoot, true);
            gemStone.RigidBody.isKinematic = false;
        }

        private bool PerformGemRetrieval()
        {
            Vector3 retrievePosition = m_GemHoldingRoot.position;
            Quaternion retrieveRotation = Quaternion.identity;

            return m_PickedGemStone.transform.MoveTowards(retrievePosition, retrieveRotation, m_Descriptor.RetrieveSpeed, m_Time.DeltaTime);
        }

        private bool PerformGemRelease()
        {
            Vector3 releasePosition = m_GemHoldingRoot.position + m_GemHoldingRoot.rotation * m_Descriptor.ReleaseOffset;
            Quaternion releaseRotation = Quaternion.Euler(m_Descriptor.ReleaseAngle);

            return m_PickedGemStone.transform.MoveTowards(releasePosition, releaseRotation, m_Descriptor.ReleaseSpeed, m_Time.DeltaTime);
        }
        #endregion
    }
}
