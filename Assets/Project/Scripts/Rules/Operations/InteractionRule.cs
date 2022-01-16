using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using Swap.Data.Descriptors;
using Swap.Data.Models;
using Swap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Rules.Operations
{
    [RuleAccess(typeof(IInteractionRule))]
    public class InteractionRule : GameRule, IInteractionRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        private InteractDescriptor m_Descriptor;

        private LevelState m_LevelState;
        private bool m_FoundInteraction;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<InteractDescriptor>("Interaction");

            m_LevelState = LevelRule.GetLevelState();
            m_FoundInteraction = false;

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update() 
        {
            m_FoundInteraction = false;
        }
        #endregion

        #region IInteractionRule API
        public bool CheckCurrentInteraction<T>(GetCurrentElement<T> elementGetter, out T currentElement) where T : MonoBehaviour
        {
            currentElement = default;
            if (m_FoundInteraction)
                return false;

            if (m_LevelState.CurrentRobotBody == null)
                return false;

            currentElement = elementGetter(m_LevelState);
            if (currentElement == null)
                return false;

            m_FoundInteraction = true;
            return true;
        }

        public bool FindEligibleInteraction<T>(IEnumerable<T> elements, GetInteractivity<T> interactionGetter, out T eligibleElement) where T : MonoBehaviour
        {
            eligibleElement = default;
            if (m_FoundInteraction)
                return false;

            if (m_LevelState.CurrentRobotBody == null)
                return false;

            float minDistance = float.MaxValue;
            Vector3 interactPosition = m_LevelState.CurrentRobotBody.InteractRoot.position;

            foreach (T element in elements)
            {
                Interactivity interactivity = interactionGetter.Invoke(element);
                Vector3 elementPosition = element.transform.position + element.transform.rotation * interactivity.CenterOffset;

                float horizontalDistance = Vector3.Distance(new Vector3(elementPosition.x, interactPosition.y, elementPosition.z), interactPosition);
                if (horizontalDistance > m_Descriptor.CheckRadius)
                    continue;

                float verticalDistance = Mathf.Abs(elementPosition.y - interactPosition.y);
                if (verticalDistance > m_Descriptor.CheckHeight)
                    continue;

                if (!Physics.Raycast(interactPosition, elementPosition - interactPosition, out RaycastHit hit, float.MaxValue, m_Descriptor.CheckRaycastLayers)
                    || hit.collider != interactivity.Collider)
                    continue;

                if (horizontalDistance < minDistance)
                {
                    minDistance = horizontalDistance;
                    eligibleElement = element;
                    m_FoundInteraction = true;
                }
            }

            return eligibleElement != null;
        }
        #endregion
    }
}
