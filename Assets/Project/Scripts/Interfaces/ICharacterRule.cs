using UnityEngine;

namespace Swap.Interfaces
{
    public interface ICharacterRule
    {
        void EnterCharacter(GameObject character);

        void ExitCharacter();
    }
}
