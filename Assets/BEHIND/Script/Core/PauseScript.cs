using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject menuPauseUI; 
    
    private bool estEnPause = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estEnPause)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ContinueGame()
    {
        menuPauseUI.SetActive(false); 
        Debug.Log("Je Reprend");
        Time.timeScale = 1f;          
        estEnPause = false;
    }

    public void PauseGame()
    {
        menuPauseUI.SetActive(true);  
        Debug.Log("J'arrête");
        Time.timeScale = 0f;          
        estEnPause = true;
    }

    public void QuitGame()
    {
        Application.Quit ();
    Debug.Log("Je Quitte");
    }
}
