using UnityEngine;

public class Player : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private float speed = 5f;
    #endregion

    #region Update
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
    #endregion
}
