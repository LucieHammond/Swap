using GameEngine.Core.Unity.Utilities;
using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Components;
using Swap.Components.States;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Linq;
using UnityEngine;

namespace Swap.Rules.Mechanics
{
    public class GemReceptacleRule : GameRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILogicRule LogicRule;

        private ReceptionDescriptor m_Descriptor;

        private GemStone[] m_GemStones;
        private GemReceptacle[] m_GemReceptacles;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<ReceptionDescriptor>("GemReception");

            m_GemStones = LevelRule.GetGemStones();
            m_GemReceptacles = LevelRule.GetGemReceptacles();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            foreach (GemReceptacle receptacle in m_GemReceptacles)
            {
                bool wasActivated = receptacle.IsActivated;
                if (receptacle.ConnectedStone != null)
                    CheckExistingConnection(receptacle);

                switch (receptacle.State)
                {
                    case GemReceptacleState.Waiting:
                        CheckNewConnection(receptacle);
                        break;
                    case GemReceptacleState.Attracting:
                        PerformAttraction(receptacle);
                        break;
                    case GemReceptacleState.Inserting:
                        PerformInsertion(receptacle);
                        break;
                }

                receptacle.UpdateActivation();
                if (wasActivated != receptacle.IsActivated)
                    LogicRule.UpdateStatusSignal(receptacle.SignalToSend, receptacle.IsActivated);
            }
        }
        #endregion

        #region private
        private void CheckExistingConnection(GemReceptacle receptacle)
        {
            bool lostConnection = receptacle.ConnectedStone.CurrentReceptacle != receptacle;
            if (lostConnection)
            {
                GemStone previousStone = receptacle.ConnectedStone;
                if (receptacle.State == GemReceptacleState.Holding || receptacle.State == GemReceptacleState.Inserting)
                    previousStone.transform.position = receptacle.ReceptionPoint.position + m_Descriptor.AttractionCenter;

                receptacle.DisconnectStone();
            }
        }

        private void CheckNewConnection(GemReceptacle receptacle)
        {
            bool newConnection = FindMatchingGemStone(receptacle, out GemStone matchingStone);
            if (newConnection)
            {
                receptacle.AttractStone(matchingStone);
                matchingStone.ConnectToReceptacle(receptacle);
            }
        }

        private bool FindMatchingGemStone(GemReceptacle receptacle, out GemStone matchingStone)
        {
            matchingStone = null;
            foreach (GemStone gemstone in m_GemStones.Where(stone => stone.State == GemStoneState.Free))
            {
                if (gemstone.Color != receptacle.Color)
                    continue;

                Vector3 connectionCenter = receptacle.ReceptionPoint.position + m_Descriptor.ConnectionCenter;
                if (Vector3.Distance(connectionCenter, gemstone.transform.position) <= m_Descriptor.ConnectionRadius)
                {
                    matchingStone = gemstone;
                    return true;
                }
            }

            return false;
        }

        private void PerformAttraction(GemReceptacle receptacle)
        {
            GemStone stone = receptacle.ConnectedStone;
            Vector3 attractionPosition = receptacle.ReceptionPoint.position + m_Descriptor.AttractionCenter;
            Quaternion attractionRotation = Quaternion.identity;

            if (stone.transform.MoveTowards(attractionPosition, attractionRotation, m_Descriptor.AttractionSpeed, m_Time.DeltaTime))
            {
                receptacle.InsertStone(stone);
            }
        }

        private void PerformInsertion(GemReceptacle receptacle)
        {
            GemStone stone = receptacle.ConnectedStone;
            Vector3 insertionPosition = receptacle.ReceptionPoint.position + m_Descriptor.InsertionCenter;
            Quaternion insertionRotation = Quaternion.identity;

            if (stone.transform.MoveTowards(insertionPosition, insertionRotation, m_Descriptor.InsertionSpeed, m_Time.DeltaTime))
            {
                receptacle.HoldStone(stone);
            }
        }
        #endregion
    }
}
