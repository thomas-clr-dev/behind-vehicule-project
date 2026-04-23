using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void PhysicsUpdate();
    void Exit();
}
