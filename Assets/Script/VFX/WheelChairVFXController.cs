
using UnityEngine;

public class WheelVFX : MonoBehaviour
{
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
        if(e.MotorTorque > 5)
        {
            GameObject go = Instantiate(vfxPrefab,transform.position, new Quaternion(0,0,0,0));
        }
    }
   

   private void  OnDisable()
    {
        EventBus<WheelStateDataEvent>.Deregister(dataBinding);
    }

}
