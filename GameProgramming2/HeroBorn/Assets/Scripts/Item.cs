using UnityEngine;

public class item : MonoBehaviour
{
    [Header("Speed")]
    public int rotSpeed = 100;

    Transform itemTransform;

    void Awake()
    {
        itemTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        ItmeRotation();
    }

    void ItmeRotation()
    {
        itemTransform.Rotate(rotSpeed * Time.deltaTime, 0, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Destroy(this.transform.gameObject);
            Debug.Log("Item collected");
        }
    }
}
