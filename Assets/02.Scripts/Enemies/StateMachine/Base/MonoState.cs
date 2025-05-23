using RobustFSM.Interfaces;
using System;
using UnityEngine;

namespace RobustFSM.Base
{
    [Serializable]
    public abstract class MonoState : MonoBehaviour, IState
    {
        public string StateName { get; internal set; }

        public IFSM Machine { get; internal set; }
        public IFSM SuperMachine { get; internal set; }

        public virtual void Initialize()
        {
            //if no name hase been specified set the name of this instance to the the
            if (String.IsNullOrEmpty(StateName))
                StateName = GetType().Name;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        public virtual string GetStateName()
        {
            return StateName;
        }
    }
}