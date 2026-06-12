using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : MonoBehaviour
{
    public int CheckpointID;
    public UnityEvent OnCheckPointSelected;

    public void SelectCheckPoint()
    {
        Debug.Log($"Checkpoint {CheckpointID} selected.");
        OnCheckPointSelected?.Invoke();
    }

    public void SetCheckPoint()
    {
        GameServiceLocator.Get<IGameManager>().SetCheckpoint(this);
    }
}