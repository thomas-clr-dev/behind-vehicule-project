using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject menuPauseUI; 
    
    private bool IsPaused = false;

    private IInputManager inputManager;


    private void Start()
    {
       inputManager = GameServiceLocator.Get<IInputManager>();
    }

    void Update()
    {
        if (inputManager.IsPausePressed)
        {
            if (IsPaused)
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
        IsPaused = false;
    }

    public void PauseGame()
    {
        menuPauseUI.SetActive(true);  
        Debug.Log("J'arrête");
        Time.timeScale = 0f;          
        IsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit ();
        Debug.Log("Je Quitte");
    }

    public void ActivateEnemy()
    {
        GameEngineEvent.Trigger(GameEngineEventTypes.ActivateEnemy);
    }

    public void DeactivateEnemy()
    {
        GameEngineEvent.Trigger(GameEngineEventTypes.DeactivateEnemy);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartScreen");
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

}
