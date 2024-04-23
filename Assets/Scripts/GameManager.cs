using UnityEngine;

public class GameManager : MonoBehaviour // służy do zapamiętania wybranego poziomu trudności
{
    public static GameManager instance;
    public int selectedDifficulty; // 0 - łatwy 1 - trudny

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // instancja tego obiektu nadal będzie istnieć, mimo zmiany scen
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeDifficulty(int diff) // funkcja do zmiany poziomu trudności
    {
        this.selectedDifficulty = diff;
    }
}
