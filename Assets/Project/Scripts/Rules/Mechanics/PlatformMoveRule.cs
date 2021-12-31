using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using Swap.Components;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class PlatformMoveRule : GameRule
    {
        private class PlatformInfo
        {
            public Vector3 PositionA;
            public Vector3 RotationA;

            public Vector3 PositionB;
            public Vector3 RotationB;
        }

        private enum PlatformState
        {
            InPositionA,
            InPositionB,
            MovingFromAToB,
            MovingFromBToA
        }

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        private MobilePlatform[] m_MobilePlatforms;

        private Dictionary<int, PlatformInfo> m_InitialInfos;
        private Dictionary<int, PlatformState> m_CurrentStates;

        public PlatformMoveRule()
        {
            m_InitialInfos = new Dictionary<int, PlatformInfo>();
            m_CurrentStates = new Dictionary<int, PlatformState>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_MobilePlatforms = LevelRule.GetMobilePlatforms();

            for (int i = 0; i < m_MobilePlatforms.Length; i++)
            {
                PlatformInfo initialInfo = new PlatformInfo()
                {
                    PositionA = m_MobilePlatforms[i].transform.position,
                    RotationA = m_MobilePlatforms[i].transform.eulerAngles,
                    PositionB = m_MobilePlatforms[i].transform.position + m_MobilePlatforms[i].TranslationalMotion,
                    RotationB = m_MobilePlatforms[i].transform.eulerAngles + m_MobilePlatforms[i].RotationalMotion
                };

                m_InitialInfos.Add(i, initialInfo);
                m_CurrentStates.Add(i, PlatformState.InPositionA);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_InitialInfos.Clear();
            m_CurrentStates.Clear();

            MarkUnloaded();
        }

        protected override void Update()
        {
            for (int i = 0; i < m_MobilePlatforms.Length; i++)
            {
                bool signalValue = LogicRule.IsActiveAll(m_MobilePlatforms[i].SignalsToListen);

                switch (m_MobilePlatforms[i].ActivationType)
                {
                    case (ActivationType.TriggerSwitch):
                        UpdateWithInstantSwitch(i, signalValue);
                        break;
                    case (ActivationType.MatchPosition):
                        UpdateWithStatusMatching(i, signalValue);
                        break;
                    case (ActivationType.ControlMotion):
                        UpdateWithMovementBlocking(i, signalValue);
                        break;
                    case (ActivationType.LaunchOneWayChange):
                        UpdateWithPermanentCompletion(i, signalValue);
                        break;
                }
            }
        }
        #endregion

        #region private
        private void UpdateWithInstantSwitch(int platformNum, bool signalValue)
        {
            if ((m_CurrentStates[platformNum] == PlatformState.InPositionA && signalValue) 
                || m_CurrentStates[platformNum] == PlatformState.MovingFromAToB)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsB(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }

            else if ((m_CurrentStates[platformNum] == PlatformState.InPositionB && signalValue)
                || m_CurrentStates[platformNum] == PlatformState.MovingFromBToA)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsA(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }
        }

        private void UpdateWithStatusMatching(int platformNum, bool signalValue)
        {
            if (m_CurrentStates[platformNum] != PlatformState.InPositionB && signalValue)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsB(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }

            else if (m_CurrentStates[platformNum] != PlatformState.InPositionA && !signalValue)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsA(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }
        }

        private void UpdateWithMovementBlocking(int platformNum, bool signalValue)
        {
            if (!signalValue)
                return;

            if (m_CurrentStates[platformNum] == PlatformState.InPositionA || m_CurrentStates[platformNum] == PlatformState.MovingFromAToB)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsB(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }

            else if (m_CurrentStates[platformNum] == PlatformState.InPositionB || m_CurrentStates[platformNum] == PlatformState.MovingFromBToA)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsA(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }
        }

        private void UpdateWithPermanentCompletion(int platformNum, bool signalValue)
        {
            if (m_CurrentStates[platformNum] == PlatformState.InPositionB)
                return;

            if ((m_CurrentStates[platformNum] == PlatformState.InPositionA && signalValue)
                || m_CurrentStates[platformNum] == PlatformState.MovingFromAToB)
            {
                m_CurrentStates[platformNum] = MovePlatformTowardsB(m_MobilePlatforms[platformNum], m_InitialInfos[platformNum]);
            }
        }

        private PlatformState MovePlatformTowardsA(MobilePlatform platform, PlatformInfo info)
        {
            Vector3 positionDiff = -platform.TranslationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            Vector3 rotationDiff = -platform.RotationalMotion * m_Time.DeltaTime / platform.MovementDuration;

            if (positionDiff.magnitude < Vector3.Distance(platform.transform.position, info.PositionA)
                || rotationDiff.magnitude < Quaternion.Angle(platform.transform.rotation, Quaternion.Euler(info.RotationA)))
            {
                platform.transform.position += positionDiff;
                platform.transform.eulerAngles += rotationDiff;
                return PlatformState.MovingFromBToA;
            }
            else
            {
                platform.transform.position = info.PositionA;
                platform.transform.eulerAngles = info.RotationA;
                return PlatformState.InPositionA;
            }
        }

        private PlatformState MovePlatformTowardsB(MobilePlatform platform, PlatformInfo info)
        {
            Vector3 positionDiff = platform.TranslationalMotion * m_Time.DeltaTime / platform.MovementDuration;
            Vector3 rotationDiff = platform.RotationalMotion * m_Time.DeltaTime / platform.MovementDuration;

            if (positionDiff.magnitude < Vector3.Distance(platform.transform.position, info.PositionB)
                || rotationDiff.magnitude < Quaternion.Angle(platform.transform.rotation, Quaternion.Euler(info.RotationB)))
            {
                platform.transform.position += positionDiff;
                platform.transform.eulerAngles += rotationDiff;
                return PlatformState.MovingFromAToB;
            }
            else
            {
                platform.transform.position = info.PositionB;
                platform.transform.eulerAngles = info.RotationB;
                return PlatformState.InPositionB;
            }
        }
        #endregion
    }
}
