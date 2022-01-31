using GameEngine.PMR.Modules;
using GameEngine.PMR.Process;
using Swap.Setups.Modes;
using System.Collections.Generic;

namespace Swap.Setups
{
    public class MainProcessSetup : IGameProcessSetup
    {
        public string Name => "MainProcess";

        public IGameModuleSetup GetServiceSetup()
        {
            return new ServicesSetup();
        }

        public List<IGameModuleSetup> GetFirstGameModes()
        {
            return new List<IGameModuleSetup>()
            {
                new PlayModeSetup()
            };
        }
    }
}
