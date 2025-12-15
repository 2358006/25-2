using UnityEngine;

public class ClearBlock : MonoBehaviour
{
    float posMin = -13f;
    float posMax = 13f;

    int level;

    void Start()
    {
        InitPos();
    }

    public void InitPos()
    {
        level = GameManager.instance.level;

        if (level > 5) { transform.localScale *= Random.Range(0f, 1f); } //  Lv 5 이상일 때 Clear Block 크기 랜덤
        transform.position = new Vector3(Random.Range(posMin, posMax), transform.position.y, Random.Range(posMin, posMax));
        this.gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        GameManager.instance.GameClear();
    }
}
