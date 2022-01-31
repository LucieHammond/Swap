using GameEngine.PMR.Process;
using GameEngine.PMR.Unity.Process;
using Swap.Setups;
using UnityEngine;

namespace Swap
{
    public class SwapRoot : ApplicationRoot
    {
        protected override IGameProcessSetup GetProcessSetup()
        {
            return new MainProcessSetup();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
