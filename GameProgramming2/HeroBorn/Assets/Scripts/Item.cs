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
}
