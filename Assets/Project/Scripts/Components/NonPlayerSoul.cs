using UnityEngine;

namespace Swap.Components
{
    public class NonPlayerSoul : MonoBehaviour
    {
        [Header("Components")]
        public GameObject Highlight;

        public Collider Collider;

        #region Component methods
        public NonPlayerSoul Setup()
        {
            return this;
        }
        #endregion
    }
}
