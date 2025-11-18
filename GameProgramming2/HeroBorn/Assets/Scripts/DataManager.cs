using UnityEngine;
public class DataManager : MonoBehaviour, IManager
{
    string state;
    public string State
    {
        get { return state; }
        set { state = value; }
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        state = "Data manager initialize";
        Debug.Log(state);
    }
}
