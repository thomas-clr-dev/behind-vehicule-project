public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void Initialize(IState newState)
    {
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void ChangeState(IState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void UpdateState()
    {
        CurrentState?.Update();
    }

    public void PhysicsUpdateState()
    {
        CurrentState?.PhysicsUpdate();
    }

    public IState GetCurrentState()
    {
        return CurrentState;
    }

    public void ExitCurrentState()
    {
        CurrentState?.Exit();
        CurrentState = null;
    }   
}
