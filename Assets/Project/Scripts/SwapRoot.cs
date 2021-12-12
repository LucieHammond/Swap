using GameEngine.PMR.Process;
using GameEngine.PMR.Unity.Process;
using Swap.Setups;

namespace Swap
{
    public class SwapRoot : ApplicationRoot
    {
        protected override IGameProcessSetup GetProcessSetup()
        {
            return new MainProcessSetup();
        }
    }
}