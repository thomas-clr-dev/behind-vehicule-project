using UnityEngine;

public interface IGameManager
{
    void SetCheckpoint(CheckPoint checkpoint);
    int GetCheckpointID();

    void ResetGame(); 
    void HasSeenTutorial(bool seen);
    bool GetHasSeenTutorial();
}
