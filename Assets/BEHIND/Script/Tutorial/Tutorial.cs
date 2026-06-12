using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public GameObject panel;
    private static bool hasSeenTutorial;

    private void OnEnable()
    {
        if (hasSeenTutorial)
        {
            panel.SetActive(false);
        }
    }
    private void Start()
    {
        bool value = GameServiceLocator.Get<IGameManager>().GetHasSeenTutorial();
        if (value == true)
        {
            panel.SetActive(false);
            hasSeenTutorial = true;
        }
        else
        {
            panel.SetActive(true);
            hasSeenTutorial = false;
        }
    }
    public void UpdateTutorial()
    {
        GameServiceLocator.Get<IGameManager>().HasSeenTutorial(true);
    }
}
