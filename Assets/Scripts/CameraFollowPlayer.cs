using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform target;
    public float speed;

    private void Start()
    {
        if(target == null)  
        {
            Vector3 StartPosition = target.position;
            StartPosition.z = -10f;
            transform.position = StartPosition;
        }     
    }
    private void FixedUpadte()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.fixedDeltaTime);
    }
}
 