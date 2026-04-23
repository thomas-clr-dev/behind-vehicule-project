using System;
using System.Collections.Generic;

public interface IStateController
{
    Dictionary<Type, IState> States { get; }

    void ChangeState<T>() where T : IState;
}