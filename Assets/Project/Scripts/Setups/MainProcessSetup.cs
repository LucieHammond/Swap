using GameEngine.PMR.Modules;
using GameEngine.PMR.Process;
using System.Collections.Generic;

public class MainProcessSetup : IGameProcessSetup
{
    public string Name => "MainProcess";

    public IGameModuleSetup GetServiceSetup()
    {
        return new ServiceModuleSetup();
    }

    public List<IGameModuleSetup> GetFirstGameModes()
    {
        return new List<IGameModuleSetup>()
        {
            new LevelModuleSetup()
        };
    }
}
