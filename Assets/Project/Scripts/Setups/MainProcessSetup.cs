using GameEngine.PMR.Modules;
using GameEngine.PMR.Process;
using Swap.Setups.Modules;
using System.Collections.Generic;

namespace Swap.Setups
{
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
}
