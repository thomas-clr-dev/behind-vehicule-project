using UnityEngine;

public interface IGameManager
{
    void SetCheckpoint(CheckPoint checkpoint);
    CheckPoint GetCurrentCheckpoint();
}
