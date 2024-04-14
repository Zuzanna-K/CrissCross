using UnityEngine;
using UnityEngine.UI;

public class DifficultButton : MonoBehaviour
{
    public int difficultyLevel;
    public Button easyButton;

    private void Start()
    {
        // Domyślnie ustawiaj trudny poziom trudności jako wybrany
        difficultyLevel = 1;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        GameManager.instance.ChangeDifficulty(difficultyLevel);
    }
}
