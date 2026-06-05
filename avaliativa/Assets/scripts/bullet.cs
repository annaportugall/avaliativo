using UnityEngine;

public class bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision){
        Destroy(gameObject);
    }
}
