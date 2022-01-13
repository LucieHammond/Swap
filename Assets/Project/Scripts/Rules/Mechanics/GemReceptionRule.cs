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

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        private ReceptionDescriptor m_Descriptor;

        private GemStone[] m_GemStones;
        private GemReceptacle[] m_GemReceptacles;
        private Transform m_GemsRoot;

        private Dictionary<GemReceptacle, GemStone> m_CurrentConnections;
        private Dictionary<GemReceptacle, ReceptionState> m_CurrentStates;
        private Dictionary<GemReceptacle, bool> m_Activations;

        public GemReceptionRule()
        {
            m_CurrentConnections = new Dictionary<GemReceptacle, GemStone>();
            m_CurrentStates = new Dictionary<GemReceptacle, ReceptionState>();
            m_Activations = new Dictionary<GemReceptacle, bool>();
        }

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<ReceptionDescriptor>("GemReception");

            m_GemStones = LevelRule.GetGemStones();
            m_GemReceptacles = LevelRule.GetGemReceptacles();
            m_GemsRoot = LevelRule.GetRootTransform("Objects");

            foreach (GemReceptacle receptacle in m_GemReceptacles)
            {
                m_CurrentConnections.Add(receptacle, null);
                m_CurrentStates.Add(receptacle, ReceptionState.Waiting);
                m_Activations.Add(receptacle, false);
            }

            MarkInitialized();
        }

        protected override void Unload()
        {
            m_CurrentConnections.Clear();
            m_CurrentStates.Clear();
            m_Activations.Clear();

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
            foreach (GemReceptacle receptacle in m_GemReceptacles)
            {
                if (m_CurrentConnections[receptacle] != null)
                {
                    bool lostConnection = m_CurrentConnections[receptacle].transform.parent != receptacle.ObjectRoot;
                    if (lostConnection)
                    {
                        OnConnectionLost(m_CurrentStates[receptacle], receptacle, m_CurrentConnections[receptacle]);

                        m_CurrentConnections[receptacle] = null;
                        m_CurrentStates[receptacle] = ReceptionState.Waiting;
                    }
                }
                else
                {
                    bool newConnection = FindMatchingGemStone(receptacle, out GemStone matchingStone);
                    if (newConnection)
                    {
                        matchingStone.transform.SetParent(receptacle.ObjectRoot, true);
                        matchingStone.RigidBody.isKinematic = true;

                        m_CurrentConnections[receptacle] = matchingStone;
                        m_CurrentStates[receptacle] = ReceptionState.Attracting;
                    }
                }
            }
        }

        private bool FindMatchingGemStone(GemReceptacle receptacle, out GemStone matchingStone)
        {
            matchingStone = null;
            foreach (GemStone gemstone in m_GemStones)
            {
                if (gemstone.transform.parent != m_GemsRoot)
                    continue;

                if (receptacle.Color != gemstone.Color)
                    continue;

                Vector3 connectionCenter = receptacle.ObjectRoot.position + m_Descriptor.ConnectionCenter;
                if (Vector3.Distance(connectionCenter, gemstone.transform.position) <= m_Descriptor.ConnectionRadius)
                {
                    matchingStone = gemstone;
                    return true;
                }
            }
            
            return false;
        }

        private void OnConnectionLost(ReceptionState previousState, GemReceptacle receptacle, GemStone stone)
        {
            if (previousState == ReceptionState.Holding || previousState == ReceptionState.Inserting)
                stone.transform.position = receptacle.ObjectRoot.position + m_Descriptor.AttractionCenter;
        }

        private void PerformAttractions()
        {
            foreach (GemReceptacle receptacle in m_GemReceptacles)
            {
                if (m_CurrentStates[receptacle] == ReceptionState.Attracting)
                {
                    GemStone gemStone = m_CurrentConnections[receptacle];
                    Vector3 attractionPosition = receptacle.ObjectRoot.position + m_Descriptor.AttractionCenter;
                    Quaternion attractionRotation = Quaternion.identity;

                    if (gemStone.transform.MoveTowards(attractionPosition, attractionRotation, m_Descriptor.AttractionSpeed, m_Time.DeltaTime))
                    {
                        m_CurrentStates[receptacle] = ReceptionState.Inserting;
                    }
                }
            }
        }

        private void PerformInsertions()
        {
            foreach (GemReceptacle receptacle in m_GemReceptacles)
            {
                if (m_CurrentStates[receptacle] == ReceptionState.Inserting)
                {
                    GemStone gemStone = m_CurrentConnections[receptacle];
                    Vector3 insertionPosition = receptacle.ObjectRoot.position;
                    Quaternion insertionRotation = Quaternion.identity;

                    if (gemStone.transform.MoveTowards(insertionPosition, insertionRotation, m_Descriptor.InsertionSpeed, m_Time.DeltaTime))
                    {
                        m_CurrentStates[receptacle] = ReceptionState.Holding;
                    }
                }
            }
        }

        private void CheckActivations()
        {
            foreach (GemReceptacle receptacle in m_GemReceptacles)
            {
                bool activated = IsActivated(receptacle);

                if (activated && !m_Activations[receptacle])
                {
                    m_Activations[receptacle] = true;
                    LogicRule.UpdateStatusSignal(receptacle.SignalToSend, true);
                }
                else if (!activated && m_Activations[receptacle])
                {
                    m_Activations[receptacle] = false;
                    LogicRule.UpdateStatusSignal(receptacle.SignalToSend, false);
                }
            }
        }

        private bool IsActivated(GemReceptacle gemReceptacle)
        {
            if (m_CurrentConnections[gemReceptacle] == null)
                return false;

            GemStone gemStone = m_CurrentConnections[gemReceptacle];

            return gemReceptacle.Color == gemStone.Color 
                && Vector3.Distance(gemReceptacle.ObjectRoot.position, gemStone.transform.position) < m_Descriptor.ActivationRadius;
        }
        #endregion
    }
}
