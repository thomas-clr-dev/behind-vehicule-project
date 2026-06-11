using UnityEngine;
using UnityEngine.Events;

public class CheckPoint : MonoBehaviour
{
    public UnityEvent OnCheckPointSelected;

    public void SelectCheckPoint()
    {
        OnCheckPointSelected?.Invoke();
    }

    public void SetCheckPoint()
    {
        GameServiceLocator.Get<IGameManager>().SetCheckpoint(this);
    }
}
