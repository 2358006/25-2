using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    Text healthText;
    Text itemText;
    Text progressText;
    Button WinButton;

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
                progressText.text = "You've found all the items!";
                WinButton.gameObject.SetActive(true);

                Time.timeScale = 0f;
            }
            else { progressText.text = $"Item found, only {maxItems - itemsCollected} more to go!"; }
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

    void Awake()
    {
        healthText = GameObject.Find("Health").GetComponent<Text>();
        itemText = GameObject.Find("Items").GetComponent<Text>();
        progressText = GameObject.Find("Progress").GetComponent<Text>();
        WinButton = GameObject.Find("WinButton").GetComponent<Button>();
    }

    void Start()
    {
        itemText.text = $"Items : {items}";
        healthText.text = $"Life : {playerHp}";
        WinButton.gameObject.SetActive(false);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}