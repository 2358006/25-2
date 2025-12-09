using UnityEngine;

public class item : MonoBehaviour
{
    GameManager gameManager;
    Transform itemTransform;

    [Header("Speed")]
    public int rotSpeed = 100;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        itemTransform = this.GetComponent<Transform>();
    }

    void Update()
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

        gameManager.items += 1;
    }
}
