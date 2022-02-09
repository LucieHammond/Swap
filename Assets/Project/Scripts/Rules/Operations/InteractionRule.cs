using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Content;
using GameEngine.PMR.Unity.Rules;
using GameEngine.PMR.Unity.Rules.Dependencies;
using Swap.Components;
using Swap.Components.Template;
using Swap.Data.Descriptors;
using Swap.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Swap.Rules.Operations
{
    [RuleAccess(typeof(IInteractionRule))]
    public class InteractionRule : GameRule, ISceneGameRule, IInteractionRule
    {
        public HashSet<string> RequiredScenes => new HashSet<string>() { "InteractionUI" };

        [ObjectDependency("InteractionInterface", ObjectDependencyElement.GameObject, true)]
        public GameObject m_InteractionRoot;

        [RuleDependency(RuleDependencySource.Service, true)]
        public IDescriptorContentService ContentService;

        [RuleDependency(RuleDependencySource.SameModule, true)]
        public ILevelRule LevelRule;

        private InteractDescriptor m_Descriptor;

        private PlayerSoul m_PlayerSoul;
        private GameObject m_Marker;
        private Animator m_MarkerAnimator;

        private bool m_FoundInteraction;
        private bool m_IsMarkedInteraction;
        private bool m_IsNewInteraction;
        private GameObject m_InteractiveElement;
        private Vector3 m_MarkerPosition;

        #region GameRule cycle
        protected override void Initialize()
        {
            m_Descriptor = ContentService.GetContentDescriptor<InteractDescriptor>("Interaction");

            m_PlayerSoul = LevelRule.GetPlayerSoul();
            m_FoundInteraction = false;
            m_Marker = m_InteractionRoot.transform.Find("Marker").gameObject;
            m_MarkerAnimator = m_Marker.GetComponent<Animator>();

            MarkInitialized();
        }

        protected override void Unload()
        {
            MarkUnloaded();
        }

        protected override void Update()
        {
            if (!m_FoundInteraction)
                m_InteractiveElement = null;

            if (m_FoundInteraction && m_IsMarkedInteraction)
            {
                m_Marker.transform.position = m_MarkerPosition;
                m_Marker.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
                m_Marker.SetActive(true);

                if (m_IsNewInteraction)
                    m_MarkerAnimator.SetTrigger("Pop");
            }
            else
            {
                m_Marker.SetActive(false);
            }

            m_FoundInteraction = false;
            m_IsMarkedInteraction = false;
            m_IsNewInteraction = false;
        }
        #endregion

        #region IInteractionRule API
        public bool CheckPriorityInteraction<T>(GetPlayerElement<T> elementGetter, out T currentElement) where T : MonoBehaviour, IInteractive
        {
            currentElement = default;
            if (m_FoundInteraction)
                return false;

            if (m_PlayerSoul.CurrentRobotBody == null)
                return false;

            currentElement = elementGetter(m_PlayerSoul);
            if (currentElement == null || !currentElement.IsAvailable())
                return false;

            RegisterInteractionForFrame(currentElement.gameObject, false);
            return true;
        }

        public bool FindEligibleInteraction<T>(IEnumerable<T> elements, out T eligibleElement) where T : MonoBehaviour, IInteractive
        {
            eligibleElement = default;
            if (m_FoundInteraction)
                return false;

            if (m_PlayerSoul.CurrentRobotBody == null)
                return false;

            float minDistance = float.MaxValue;
            Vector3 markerPosition = Vector3.zero;
            Vector3 interactPosition = m_PlayerSoul.CurrentRobotBody.InteractPoint.position;

            foreach (T element in elements.Where((element) => element.IsAvailable()))
            {
                Vector3 elementPosition = element.GetInteractionCenter();

                float horizontalDistance = Vector3.Distance(new Vector3(elementPosition.x, interactPosition.y, elementPosition.z), interactPosition);
                if (horizontalDistance > m_Descriptor.CheckRadius)
                    continue;

                float verticalDistance = Mathf.Abs(elementPosition.y - interactPosition.y);
                if (verticalDistance > m_Descriptor.CheckHeight)
                    continue;

                if (!Physics.Raycast(interactPosition, elementPosition - interactPosition, out RaycastHit hit, float.MaxValue, m_Descriptor.CheckRaycastLayers)
                    || !element.ReceiveInteractionRay(hit))
                    continue;

                if (horizontalDistance < minDistance)
                {
                    minDistance = horizontalDistance;
                    eligibleElement = element;
                    markerPosition = element.GetMarkerPosition();
                }
            }

            if (eligibleElement == null)
                return false;

            RegisterInteractionForFrame(eligibleElement.gameObject, true, markerPosition);
            return true;
        }


        #endregion

        #region private
        private void RegisterInteractionForFrame(GameObject element, bool marked, Vector3? markerPosition = null)
        {
            m_FoundInteraction = true;
            m_IsMarkedInteraction = marked;
            m_IsNewInteraction = element != m_InteractiveElement;

            m_InteractiveElement = element;
            m_MarkerPosition = markerPosition.GetValueOrDefault();
        }
        #endregion
    }
}
