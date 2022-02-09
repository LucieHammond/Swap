using Swap.Data.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Swap.Components
{
    public class Door : MonoBehaviour
    {
        [Header("Configuration")]
        public List<Signal> SignalsToListen;

        [Header("Components")]
        public Animator Animator;

        [Header("Parameters")]
        public string OpenAnimBool;


        #region Component properties
        public bool IsOpen { get; private set; }
        #endregion

        #region Component methods
        public Door Setup()
        {
            IsOpen = false;
            return this;
        }

        public void Open()
        {
            IsOpen = true;
            Animator.SetBool(OpenAnimBool, true);
        }

        public void Close()
        {
            IsOpen = false;
            Animator.SetBool(OpenAnimBool, false);
        }
        #endregion
    }
}
