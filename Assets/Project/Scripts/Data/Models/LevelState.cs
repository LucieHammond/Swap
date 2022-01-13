using Swap.Components;
using System.Collections.Generic;

namespace Swap.Data.Models
{
    public class LevelState
    {
        public RobotBody CurrentRobotBody;

        public Dictionary<RobotBody, GemStone> CurrentlyHeldGems;
    }
}
