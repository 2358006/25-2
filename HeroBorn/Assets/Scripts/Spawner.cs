using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 소환 범위
    public float posMin = -13f;
    public float posMax = 13f;

    [Header("Enemys")]
    public GameObject enemy;

    [Header("Items")]
    public GameObject item;
    public int min = 1;
    public int max = 5;

    void Start()
    {
        int level = GameManager.instance.level;

        SpawnEnemy(level);
        SpawnItem();
    }
    internal void SpawnEnemy(int lv)
    {
        int getEnemy = GameManager.instance.maxEnemys[lv];

        for (int i = 0; i < getEnemy; i++)
        {
            Vector3 pos = new Vector3(Random.Range(posMin, posMax), 0.5f, Random.Range(posMin, posMax));
            Instantiate(enemy, pos, transform.rotation);
        }
    }

    internal void SpawnItem()
    {
        int getItem = Random.Range(min, max);
        for (int i = 0; i < getItem; i++)
        {
            Vector3 pos = new Vector3(Random.Range(posMin, posMax), 0.5f, Random.Range(posMin, posMax));
            Instantiate(item, pos, transform.rotation);
        }
    }
}
