using UnityEngine;

public interface IGameManager
{
    void SetCheckpoint(CheckPoint checkpoint);
    int GetCheckpointID();
}
