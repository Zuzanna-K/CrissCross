using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int selectedDifficulty; // 0 - łatwy 1 - trudny

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeDifficulty(int diff)
    {
        this.selectedDifficulty = diff;
    }
}
