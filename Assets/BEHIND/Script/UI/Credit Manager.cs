using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("Prod");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
