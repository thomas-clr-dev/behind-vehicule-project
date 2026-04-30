

using UnityEngine;

public class WheelVFX : MonoBehaviour

{
    private bool vfxAlreadyPlayed = false;

    private EventBinding<WheelStateDataEvent> dataBinding;

    [SerializeField] private GameObject vfxPrefab;
    private void OnEnable()
    {
        dataBinding = new EventBinding<WheelStateDataEvent>(OnDataUpdated);
        EventBus<WheelStateDataEvent>.Register(dataBinding);
    }


    private void OnDataUpdated(WheelStateDataEvent e)
    {
        Debug.Log(e.MotorTorque);
        if(e.MotorTorque > 5 && !vfxAlreadyPlayed)
        {
            GameObject go = Instantiate(vfxPrefab, transform.position, transform.rotation);

            vfxAlreadyPlayed = true;
        }

        else if (e.MotorTorque <1)
        {
            vfxAlreadyPlayed = false;
        }
    }
   

   private void  OnDisable()
    {
        EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}
