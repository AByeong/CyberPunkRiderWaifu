using UnityEngine;

namespace RobustFSM.Interfaces
{
    public interface IState
    {
        string StateName { get; }

        IFSM Machine { get; }
        IFSM SuperMachine { get; }

        string GetStateName();

        void Initialize();
        void OnEnter();
        void OnExit();
    }
}