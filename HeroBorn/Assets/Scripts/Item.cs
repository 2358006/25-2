using UnityEngine;

public class item : MonoBehaviour
{
    Transform itemTransform;

    [Header("Speed")]
    public int rotSpeed = 100;

    void Awake()
    {
        itemTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        itemTransform.Rotate(rotSpeed * Time.deltaTime, 0, 0);

        if (GameManager.instance.isGameFinished) { Destroy(this.transform.gameObject); }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Destroy(this.transform.gameObject);
            Debug.Log("Item ate");
        }

        GameManager.instance.UpdateScene("You get a item");
        GameManager.instance.PlusHp();
    }
}
