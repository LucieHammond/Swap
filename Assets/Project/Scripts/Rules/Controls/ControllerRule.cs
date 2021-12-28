using GameEngine.PMR.Rules;
using GameEngine.PMR.Rules.Dependencies;
using GameEngine.PMR.Unity.Basics.Input;
using Swap.Interfaces;
using UnityEngine;

namespace Swap.Rules.Controls
{
    [RuleAccess(typeof(IControllerRule))]
    public class ControllerRule : GameRule, IControllerRule
    {
        [RuleDependency(RuleDependencySource.Service, true)]
        public IInputService InputService;

        // Values
        private Vector2 m_Move;
        private Vector2 m_Look;
        private bool m_Run;
        private float m_Zoom;
        
        // Buttons
        private bool m_TriggerJump;
        private bool m_TriggerInteract;
        private bool m_TriggerSwap;
        private bool m_TriggerPause;

        #region GameRule cycle
        protected override void Initialize()
        {
            InputService.RegisterAxis2DCallback("GamePlay", "Move", OnMove);
            InputService.RegisterAxis2DCallback("GamePlay", "Look", OnLook);
            InputService.RegisterStatusCallback("GamePlay", "Run", OnRun);
            InputService.RegisterButtonCallback("GamePlay", "Jump", OnJump);
            InputService.RegisterButtonCallback("GamePlay", "Interact", OnInteract);
            InputService.RegisterButtonCallback("GamePlay", "Swap", OnSwap);
            InputService.RegisterAxis1DCallback("GamePlay", "Zoom", OnZoom);
            InputService.RegisterButtonCallback("GamePlay", "Pause", OnPause);

            MarkInitialized();
        }

        protected override void Unload()
        {
            InputService.UnregisterAxis2DCallback("GamePlay", "Move", OnMove);
            InputService.UnregisterAxis2DCallback("GamePlay", "Look", OnLook);
            InputService.UnregisterStatusCallback("GamePlay", "Run", OnRun);
            InputService.UnregisterButtonCallback("GamePlay", "Jump", OnJump);
            InputService.UnregisterButtonCallback("GamePlay", "Interact", OnInteract);
            InputService.UnregisterButtonCallback("GamePlay", "Swap", OnSwap);
            InputService.UnregisterAxis1DCallback("GamePlay", "Zoom", OnZoom);
            InputService.UnregisterButtonCallback("GamePlay", "Pause", OnPause);

            MarkUnloaded();
        }

        protected override void Update() { }

        protected override void LateUpdate()
        {
            m_TriggerJump = false;
            m_TriggerInteract = false;
            m_TriggerSwap = false;
            m_TriggerPause = false;
        }
        #endregion

        #region IControllerRule API
        public Vector2 GetMoveValue()
        {
            return m_Move;
        }

        public Vector2 GetLookValue()
        {
            
            return m_Look;
        }

        public bool IsRunning()
        {
            return m_Run;
        }

        public bool AskedJump()
        {
            return m_TriggerJump;
        }

        public bool AskedInteraction()
        {
            return m_TriggerInteract;
        }

        public bool AskedSwap()
        {
            return m_TriggerSwap;
        }

        public float GetZoomValue()
        {
            return m_Zoom;
        }

        public bool AskedPause()
        {
            return m_TriggerPause;
        }

        public void ResetInteraction()
        {
            m_TriggerInteract = false;
        }
        #endregion

        #region private
        private void OnMove(Vector2 position)
        {
            m_Move = position;
        }

        private void OnLook(Vector2 rotation)
        {
            m_Look = rotation;
        }

        private void OnRun(bool pressed)
        {
            m_Run = pressed;
        }

        private void OnJump()
        {
            m_TriggerJump = true;
        }

        private void OnInteract()
        {
            m_TriggerInteract = true;
        }

        private void OnSwap()
        {
            m_TriggerSwap = true;
        }

        private void OnZoom(float zoom)
        {
            m_Zoom = zoom;
        }

        private void OnPause()
        {
            m_TriggerPause = true;
        }
        #endregion
    }
}
