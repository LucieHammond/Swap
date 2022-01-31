using Swap.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Components
{
    public class MobilePlatform : MonoBehaviour
    {
        public ActivationType ActivationType;

        public List<Signal> SignalsToListen;

        public Vector3 TranslationalMotion;

        public Vector3 RotationalMotion;

        public float MovementDuration;

        public Collider Collider;
    }
}
