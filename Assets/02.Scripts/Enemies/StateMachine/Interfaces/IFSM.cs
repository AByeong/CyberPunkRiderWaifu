using RobustFSM.Base;
using UnityEngine;

namespace RobustFSM.Interfaces
{
    /// <summary>
    /// Interface for the finite state machine
    /// </summary>
    public interface IFSM
    {
        string MachineName { get; set; }


        bool ContainsState<T>() where T : MonoState;

        bool IsCurrentState<T>() where T : MonoState;

        bool IsPreviousState<T>() where T : MonoState;

        T GetCurrentState<T>() where T : MonoState;

        T GetPreviousState<T>() where T : MonoState;

        T GetState<T>() where T : MonoState;

        void AddState<T>() where T : MonoState, new();

        void ChangeState<T>() where T : MonoState;

        void RemoveState<T>() where T : MonoState;

        void SetInitialState<T>() where T : MonoState;

        void GoToPreviousState();

        void RemoveAllStates();
    }
}