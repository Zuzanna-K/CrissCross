using UnityEngine;
using UnityEngine.UI;

public class EasyButton : MonoBehaviour
{
    public int difficultyLevel;
    public Button difficultButton;

    private void Start()
    {
        // Domyślnie ustawiaj łatwy poziom trudności jako wybrany
        difficultyLevel = 0;
        
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        // Zapisz wybór trudności w zmiennej globalnej (np. w GameManagerze)
        GameManager.instance.ChangeDifficulty(difficultyLevel);
    }
}
