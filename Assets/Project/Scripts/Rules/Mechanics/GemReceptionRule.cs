using GameEngine.Core.Unity.Utilities;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class GemReceptionRule : GameRule
    {
        private enum ReceptionState
        {
            Waiting,
            Attracting,
            Inserting,
            Holding
        }

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        private ReceptionDescriptor m_Descriptor;

        private GemStone[] m_GemStones;
        private GemReceptacle[] m_GemReceptacles;
        private Transform m_GemsRoot;

        private Dictionary<int, int> m_CurrentConnections;
        private Dictionary<int, ReceptionState> m_CurrentStates;
        private Dictionary<int, bool> m_Activations;

        public GemReceptionRule()
        {
            m_CurrentConnections = new Dictionary<int, int>();
            m_CurrentStates = new Dictionary<int, ReceptionState>();
            m_Activations = new Dictionary<int, bool>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<ReceptionDescriptor>("GemReception");

            m_GemStones = LevelRule.GetGemStones();
            m_GemReceptacles = LevelRule.GetGemReceptacles();
            m_GemsRoot = LevelRule.GetRootTransform("Objects");

            for (int i = 0; i < m_GemReceptacles.Length; i++)
            {
                m_CurrentConnections.Add(i, -1);
                m_CurrentStates.Add(i, ReceptionState.Waiting);
                m_Activations.Add(i, false);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            UpdateConnections();
            PerformAttractions();
            PerformInsertions();
            CheckActivations();
        }
        #endregion

        #region private
        private void UpdateConnections()
        {
            for (int i = 0; i < m_GemReceptacles.Length; i++)
            {
                if (m_CurrentConnections[i] >= 0)
                {
                    bool lostConnection = m_GemStones[m_CurrentConnections[i]].transform.parent != m_GemReceptacles[i].ObjectRoot;
                    if (lostConnection)
                    {
                        OnConnectionLost(m_CurrentStates[i], m_GemReceptacles[i], m_CurrentConnections[i]);

                        m_CurrentConnections[i] = -1;
                        m_CurrentStates[i] = ReceptionState.Waiting;
                    }
                }
                else
                {
                    bool newConnection = FindMatchingGemStone(m_GemReceptacles[i], out int stoneIndex);
                    if (newConnection)
                    {
                        m_GemStones[stoneIndex].transform.SetParent(m_GemReceptacles[i].ObjectRoot, true);
                        m_GemStones[stoneIndex].RigidBody.isKinematic = true;

                        m_CurrentConnections[i] = stoneIndex;
                        m_CurrentStates[i] = ReceptionState.Attracting;
                    }
                }
            }
        }

        private bool FindMatchingGemStone(GemReceptacle receptacle, out int stoneIndex)
        {
            stoneIndex = -1;
            for (int i = 0; i < m_GemStones.Length; i++)
            {
                if (m_GemStones[i].transform.parent != m_GemsRoot)
                    continue;

                if (receptacle.Color != m_GemStones[i].Color)
                    continue;

                Vector3 connectionCenter = receptacle.ObjectRoot.position + m_Descriptor.ConnectionCenter;
                if (Vector3.Distance(connectionCenter, m_GemStones[i].transform.position) <= m_Descriptor.ConnectionRadius)
                {
                    stoneIndex = i;
                    return true;
                }
            }
            
            return false;
        }

        private void OnConnectionLost(ReceptionState previousState, GemReceptacle receptacle, int stoneIndex)
        {
            if (previousState == ReceptionState.Holding || previousState == ReceptionState.Inserting)
                m_GemStones[stoneIndex].transform.position = receptacle.ObjectRoot.position + m_Descriptor.AttractionCenter;
        }

        private void PerformAttractions()
        {
            for (int i = 0; i < m_GemReceptacles.Length; i++)
            {
                if (m_CurrentStates[i] == ReceptionState.Attracting)
                {
                    GemStone gemStone = m_GemStones[m_CurrentConnections[i]];
                    Vector3 attractionPosition = m_GemReceptacles[i].ObjectRoot.position + m_Descriptor.AttractionCenter;
                    Quaternion attractionRotation = Quaternion.identity;

                    if (gemStone.transform.MoveTowards(attractionPosition, attractionRotation, m_Descriptor.AttractionSpeed, m_Time.DeltaTime))
                    {
                        m_CurrentStates[i] = ReceptionState.Inserting;
                    }
                }
            }
        }

        private void PerformInsertions()
        {
            for (int i = 0; i < m_GemReceptacles.Length; i++)
            {
                if (m_CurrentStates[i] == ReceptionState.Inserting)
                {
                    GemStone gemStone = m_GemStones[m_CurrentConnections[i]];
                    Vector3 insertionPosition = m_GemReceptacles[i].ObjectRoot.position;
                    Quaternion insertionRotation = Quaternion.identity;

                    if (gemStone.transform.MoveTowards(insertionPosition, insertionRotation, m_Descriptor.InsertionSpeed, m_Time.DeltaTime))
                    {
                        m_CurrentStates[i] = ReceptionState.Holding;
                    }
                }
            }
        }

        private void CheckActivations()
        {
            for (int i = 0; i < m_GemReceptacles.Length; i++)
            {
                bool activated = IsActivated(i);

                if (activated && !m_Activations[i])
                {
                    m_Activations[i] = true;
                    Debug.Log($"Signal {m_GemReceptacles[i].SignalToSend}: Send value = True");
                }
                else if (!activated && m_Activations[i])
                {
                    m_Activations[i] = false;
                    Debug.Log($"Signal {m_GemReceptacles[i].SignalToSend}: Send value = False");
                }
            }
        }

        private bool IsActivated(int receptacleIndex)
        {
            if (m_CurrentConnections[receptacleIndex] < 0)
                return false;

            GemReceptacle gemReceptacle = m_GemReceptacles[receptacleIndex];
            GemStone gemStone = m_GemStones[m_CurrentConnections[receptacleIndex]];

            return gemReceptacle.Color == gemStone.Color 
                && Vector3.Distance(gemReceptacle.ObjectRoot.position, gemStone.transform.position) < m_Descriptor.ActivationRadius;
        }
        #endregion
    }
}
