using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float onScreenDelay = 5f;

    void Start()
    {
        Destroy(this.gameObject, onScreenDelay);
    }
}
