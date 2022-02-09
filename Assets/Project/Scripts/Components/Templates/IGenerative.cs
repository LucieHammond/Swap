using UnityEngine;

namespace Swap.Components.Template
{
    public interface IGenerative
    {
        void Activate();

        void Deactivate();

        Quaternion DefaultRotation();
    }
}
