using Swap.Data.Models;
using Swap.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Interfaces
{
    public delegate TElement GetPlayerElement<TElement>(PlayerSoul player);
    public delegate Interactivity GetInteractivity<TElement>(TElement element);

    public interface IInteractionRule
    {
        bool CheckPriorityInteraction<T>(GetPlayerElement<T> elementGetter, out T currentElement)
            where T : MonoBehaviour;

        bool FindEligibleInteraction<T>(IEnumerable<T> elements, GetInteractivity<T> interactionGetter, out T eligibleElement)
            where T : MonoBehaviour;
    }
}
