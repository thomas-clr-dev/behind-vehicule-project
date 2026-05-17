using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            // Cette partie sera utilisée pour le build final (PC, Mac, Mobile)
            Application.Quit();
#endif
    }
}