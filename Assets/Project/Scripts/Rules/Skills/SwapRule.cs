using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using GameEngine.PMR.Unity.Rules;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Skills
{
    public class SwapRule : GameRule, ISceneGameRule
    {
        public HashSet<string> RequiredScenes => new HashSet<string>() { "SwapUI" };

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public IControllerRule ControllerRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ICharacterRule CharacterRule;

        private SwapDescriptor m_Descriptor;

        private Camera m_Camera;
        private PlayerSoul m_PlayerSoul;
        private NonPlayerSoul[] m_NonPlayerSouls;

        private bool m_IsSwapping;
        private NonPlayerSoul m_PartnerSoul;
        private Transform m_SwapStart;
        private Transform m_SwapArrival;
        private float m_SwapDuration;
        private float m_SwapProgression;

        private Vector2 m_ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        #region GameRule API
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<SwapDescriptor>("SwapSkill");

            m_Camera = Camera.main;
            m_PlayerSoul = LevelRule.GetPlayerSoul();
            m_NonPlayerSouls = LevelRule.GetNonPlayerSouls();
            m_IsSwapping = false;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (m_IsSwapping)
            {
                bool finished = PerformSwap();
                if (finished)
                    FinishSwap();
            }
            else
            {
                int eligibleSoulIndex = GetEligibleSoul();

                UpdateSoulsHighlights(eligibleSoulIndex);
                
                if (eligibleSoulIndex >= 0 && ControllerRule.AskedSwap())
                {
                    InitiateSwap(m_NonPlayerSouls[eligibleSoulIndex]);
                }
            }
        }
        #endregion

        #region private
        private int GetEligibleSoul()
        {
            float minOffset = float.MaxValue;
            int eligibleSoulIndex = -1;

            for (int i = 0; i < m_NonPlayerSouls.Length; i++)
            {
                Vector3 worldPosition = m_NonPlayerSouls[i].transform.position;
                Vector2 screenPoint = m_Camera.WorldToScreenPoint(worldPosition);

                float offsetToCenter = Vector2.Distance(screenPoint, m_ScreenCenter);
                if (offsetToCenter > m_Descriptor.TargetRadius)
                    continue;

                float distanceToCamera = Vector3.Distance(m_Camera.transform.position, worldPosition);
                Physics.Raycast(m_Camera.ScreenPointToRay(screenPoint), out RaycastHit hit, distanceToCamera);
                if (hit.collider == null || hit.collider.gameObject != m_NonPlayerSouls[i].gameObject)
                    continue;

                if (offsetToCenter < minOffset)
                {
                    minOffset = offsetToCenter;
                    eligibleSoulIndex = i;
                }
            }

            return eligibleSoulIndex;
        }

        private void UpdateSoulsHighlights(int eligibleSoulIndex)
        {
            for (int i = 0; i < m_NonPlayerSouls.Length; i++)
            {
                if (i == eligibleSoulIndex)
                    m_NonPlayerSouls[i].Highlight.SetActive(true);
                else
                    m_NonPlayerSouls[i].Highlight.SetActive(false);
            }
        }

        private void InitiateSwap(NonPlayerSoul partnerSoul)
        {
            m_IsSwapping = true;
            m_PartnerSoul = partnerSoul;
            m_SwapStart = m_PlayerSoul.transform.parent;
            m_SwapArrival = partnerSoul.transform.parent;
            m_SwapDuration = Vector3.Distance(m_SwapStart.position, m_SwapArrival.position) / m_Descriptor.SwapSpeed;
            m_SwapProgression = 0.0f;

            partnerSoul.Highlight.SetActive(false);
            m_PlayerSoul.transform.SetParent(null, true);
            m_PartnerSoul.transform.SetParent(null, true);

            CharacterRule.ExitCharacter();
        }

        private bool PerformSwap()
        {
            m_SwapProgression = Mathf.Clamp(m_SwapProgression + m_Time.DeltaTime / m_SwapDuration, 0, 1);
            Vector3 swapBaseLine = m_SwapArrival.position - m_SwapStart.position;

            // Move player
            Vector3 playerBaseMove = swapBaseLine * m_SwapProgression;
            Vector3 playerDeviationMove = 4 * (m_Descriptor.PlayerDeviation * Vector3.up) * m_SwapProgression * (1 - m_SwapProgression);
            m_PlayerSoul.transform.position = m_SwapStart.position + playerBaseMove + playerDeviationMove;

            // Rotate player character
            Vector3 arrivalRotation = m_SwapArrival.parent.eulerAngles;
            float newArrivalAngle = Mathf.LerpAngle(arrivalRotation.y, m_PlayerSoul.transform.eulerAngles.y, m_SwapProgression);
            m_SwapArrival.parent.rotation = Quaternion.Euler(arrivalRotation.x, newArrivalAngle, 0.0f);

            // Move partner
            Vector3 partnerBaseMove = -swapBaseLine * m_SwapProgression;
            Vector3 partnerDeviationMove = 4 * (m_Descriptor.PartnerDeviation * Vector3.up) * m_SwapProgression * (1 - m_SwapProgression);
            m_PartnerSoul.transform.position = m_SwapArrival.position + partnerBaseMove + partnerDeviationMove;

            // Rotate partner character
            Vector3 startRotation = m_SwapArrival.parent.eulerAngles;
            float newStartAngle = Mathf.LerpAngle(startRotation.y, m_PartnerSoul.transform.eulerAngles.y, m_SwapProgression);
            m_SwapStart.parent.rotation = Quaternion.Euler(startRotation.x, newStartAngle, 0.0f);

            return m_SwapProgression == 1;
        }

        private void FinishSwap()
        {
            m_PlayerSoul.transform.SetParent(m_SwapArrival, true);
            m_PartnerSoul.transform.SetParent(m_SwapStart, true);

            CharacterRule.EnterCharacter(m_SwapArrival.parent.gameObject);

            m_IsSwapping = false;
            m_PartnerSoul = null;
            m_SwapStart = null;
            m_SwapArrival = null;
        }
        #endregion
    }
}
