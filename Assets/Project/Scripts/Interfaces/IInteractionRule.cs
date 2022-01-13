using Swap.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Interfaces
{
    public delegate TElement GetCurrentElement<TElement>(LevelState state);
    public delegate Interactivity GetInteractivity<TElement>(TElement element);

    public interface IInteractionRule
    {
        bool CheckCurrentInteraction<T>(GetCurrentElement<T> elementGetter, out T currentElement) 
            where T : MonoBehaviour;

        bool FindEligibleInteraction<T>(IEnumerable<T> elements, GetInteractivity<T> interactionGetter, out T eligibleElement) 
            where T : MonoBehaviour;
    }
}
