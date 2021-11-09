using GameEngine.PMR.Process;
using GameEngine.PMR.Unity.Process;

public class SwapRoot : ApplicationRoot
{
    protected override IGameProcessSetup GetProcessSetup()
    {
        return new MainProcessSetup();
    }
}
