using Swap.Components;
using Swap.Components.Template;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Interfaces
{
    public delegate T GetPlayerElement<T>(PlayerSoul player);

    public interface IInteractionRule
    {
        bool CheckPriorityInteraction<T>(GetPlayerElement<T> elementGetter, out T currentElement)
            where T : MonoBehaviour, IInteractive;

        bool FindEligibleInteraction<T>(IEnumerable<T> elements, out T eligibleElement)
            where T : MonoBehaviour, IInteractive;
    }
}
