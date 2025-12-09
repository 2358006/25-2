using UnityEngine;
using UnityEngine.UI;
using CustomExtensions;
public class GameManager : MonoBehaviour, IManager
{
    Text healthText;
    Text itemText;
    Text progressText;

    Button winButton;
    Button lostButton;

    public int maxItems = 4;
    int itemsCollected = 0;
    public int items
    {
        get { return itemsCollected; }
        set
        {
            itemsCollected = value;
            itemText.text = $"Items : {items}";

            if (itemsCollected >= maxItems)
            {
                UpdateScene("You've found all the items!");
                winButton.gameObject.SetActive(true);

                Time.timeScale = 0f;
            }
            else { UpdateScene($"Item found, only {maxItems - itemsCollected} more to go!"); }
        }
    }

    int playerHp = 10;
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
                lostButton.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                UpdateScene("Ouch... that's got hurt.");
            }
        }
    }

    string firstName;
    public string FirstName
    {
        get { return firstName; }
        set
        {
            firstName = value;
        }
    }

    string state;
    public string State
    {
        get { return state; }
        set { state = value; }
    }

    void Awake()
    {
        healthText = GameObject.Find("Health").GetComponent<Text>();
        itemText = GameObject.Find("Items").GetComponent<Text>();
        progressText = GameObject.Find("Progress").GetComponent<Text>();

        winButton = GameObject.Find("WinButton").GetComponent<Button>();
        lostButton = GameObject.Find("LostButton").GetComponent<Button>();
    }

    void Start()
    {
        itemText.text = $"Items : {items}";
        healthText.text = $"Life : {playerHp}";
        winButton.gameObject.SetActive(false);
        lostButton.gameObject.SetActive(false);
        Initialize();
    }

    public void Initialize()
    {
        state = "Game manager initialize";

        state.FancyDebug();

        Debug.Log(state);
    }

    public void RestartScene()
    {
        // Utilities.RestartLevel();
        Utilities.RestartLevel(0);
    }

    public void UpdateScene(string updateText)
    {
        progressText.text = updateText;
        Time.timeScale = 0f;
    }
}