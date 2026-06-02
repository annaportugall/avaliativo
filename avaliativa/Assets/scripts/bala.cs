using UnityEngine;

public class bala : MonoBehaviour
{
    void OnCollisionEnter(Collision collision){
        Destroy(gameObject);
    }
}
