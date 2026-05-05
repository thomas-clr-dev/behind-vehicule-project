
using UnityEngine;

public class WheelChairSFX : MonoBehaviour
{
      private EventBinding<PushEvent> dataBinding;
      
      [SerializeField] private AudioClip audioClip;

      private void Start()
    {
        dataBinding = new EventBinding<PushEvent>(OnPush);
        EventBus<PushEvent>.Register(dataBinding);
    }

    private void OnPush(PushEvent data)
    {
       Debug.Log("" + data.ToString());
    }
}
