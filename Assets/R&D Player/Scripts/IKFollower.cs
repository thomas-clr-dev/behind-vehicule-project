using UnityEngine;

public class IKFollower : MonoBehaviour
{
    [SerializeField] private Transform target;


    private void LateUpdate()
    {
        if(target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
