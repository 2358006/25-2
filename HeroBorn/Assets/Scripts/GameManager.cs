using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CustomExtensions;

public class GameManager : MonoBehaviour, IManager
{
    public static GameManager instance { get; private set; }

    Text healthText;
    Text enemyText;
    Text levelText;
    Text progressText;

    Button winButton;
    Button lostButton;
    Button levelUpButton;

    GameObject clearBlock;
    GameObject tuto;

    internal bool isGameFinished = false;
    bool isAllKill = false;

    bool isEatenItem = false;

    public int[] maxEnemys;
    public int level;

    int enemysKilled = 0;
    public int enemys
    {
        get { return enemysKilled; }
        set
        {
            enemysKilled = value;
            enemyText.text = $"Enemys : {enemys}";

            if (enemysKilled >= maxEnemys[level])
            {
                UpdateScene("You've killed all enemys");
                isAllKill = true;
            }
            else
            {
                UpdateScene($"Enemy killed, only {maxEnemys[level] - enemysKilled} more to go!");
            }
        }
    }

    public int maxHp = 10;
    int playerHp;
    public int hp
    {
        get { return playerHp; }
        set
        {
            playerHp = value;
            healthText.text = $"health : {hp}";

            if (playerHp <= 0)
            {
                UpdateScene("You want another life with that?");
                GameFail();
            }
            else if (playerHp < maxHp)
            {
                UpdateScene("Ouch... that's got hurt.");
            }
        }
    }

    string firstName;
    public string FirstName
    {
        get { return firstName; }
        set { firstName = value; }
    }

    string state;
    public string State
    {
        get { return state; }
        set { state = value; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        ComponentInitialize();
        hp = maxHp;
        Time.timeScale = 1f;
    }

    void Start()
    {
        Initialize();
        PlayerStart();
    }

    void Update()
    {
        if (isAllKill) { clearBlock.SetActive(true); }

        if (level == 0) { levelText.text = "Level : Tuto"; }
        else
        {
            tuto.SetActive(false);
            levelText.text = $"Level : {level}";
        }
    }

    public void UpdateScene(string updateText)
    {
        progressText.text = updateText;
        // Time.timeScale = 0f;
    }

    #region Button
    public void RestartScene()
    {
        // Utilities.RestartLevel();
        Utilities.RestartLevel(0);
    }

    public void LevelUp()
    {
        level++;

        hp = maxHp;
        enemys = 0;

        isGameFinished = false;
        isAllKill = false;

        Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        spawner.SpawnEnemy(level);
        spawner.SpawnItem();

        ClearBlock clear = GameObject.Find("Clear").GetComponent<ClearBlock>();
        clear.InitPos();

        PlayerStart();
        UpdateScene("Kill all the enemyss to win!");

        levelUpButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    #endregion

    #region 기능들?
    public void PlusHp()
    {
        Debug.Log("먹었다 아이템");

        if (hp < maxHp) // Hp가 max 값보다 작으면
        {
            hp += 1; // 회복
        }
        else // 아니면
        {
            Player player = GameObject.Find("Player").GetComponent<Player>();

            // 3초간  이속 1.5배
            if (!isEatenItem)
            {
                Debug.Log("빨라진다");
                player.moveSpeed *= 1.5f;
                isEatenItem = true;
                StartCoroutine(ReturnSpeed());
            }
        }
    }

    IEnumerator ReturnSpeed()
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();

        yield return new WaitForSeconds(3f);

        isEatenItem = false;
        player.moveSpeed /= 1.5f;

        Debug.Log("원상복구 되었다");

    }
    #endregion

    #region Clear
    public void GameClear()
    {
        if (isAllKill)
        {
            isGameFinished = true;
            if (level + 1 < maxEnemys.Length) { levelUpButton.gameObject.SetActive(true); }
            else { winButton.gameObject.SetActive(true); }
            Time.timeScale = 0f;
        }
    }

    public void GameFail()
    {
        isGameFinished = true;
        lostButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    #endregion

    #region Init
    public void Initialize()
    {
        state = "Game manager initialize";

        state.FancyDebug();

        Debug.Log(state);
    }

    void ComponentInitialize()
    {
        healthText = GameObject.Find("Health").GetComponent<Text>();
        healthText.text = $"Health : {playerHp}";

        enemyText = GameObject.Find("Enemys").GetComponent<Text>();
        enemyText.text = $"Enemys : {enemys}";

        levelText = GameObject.Find("Level").GetComponent<Text>();
        levelText.text = $"Level : {level}";

        progressText = GameObject.Find("Progress").GetComponent<Text>();

        winButton = GameObject.Find("WinButton").GetComponent<Button>();
        winButton.gameObject.SetActive(false);

        lostButton = GameObject.Find("LostButton").GetComponent<Button>();
        lostButton.gameObject.SetActive(false);

        levelUpButton = GameObject.Find("LevelUpButton").GetComponent<Button>();
        levelUpButton.gameObject.SetActive(false);

        clearBlock = GameObject.Find("Clear");
        clearBlock.SetActive(false);

        tuto = GameObject.Find("Tuto");
    }

    void PlayerStart()
    {
        GameObject player = GameObject.Find("Player");
        Vector3 startPos = GameObject.Find("StartPos").transform.position;
        player.transform.position = startPos;
    }
    #endregion
}