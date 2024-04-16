using UnityEngine;
using UnityEngine.UI;

public class DifficultButton : MonoBehaviour
{
    public int difficultyLevel;
    public Button easyButton;

     private ColorBlock normalColors;
     private ColorBlock grayColors; 


    private void Start()
    {
        // Domyślnie ustawiaj trudny poziom trudności jako wybrany
        difficultyLevel = 1;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        normalColors = button.colors;

        grayColors = normalColors;
        grayColors.normalColor = Color.gray; // Kolor szary
        grayColors.highlightedColor = Color.gray; // Kolor szary
        grayColors.pressedColor = Color.gray; // Kolor szary

        button.colors = grayColors ;
    }

    public void OnClick()
    {
        GameManager.instance.ChangeDifficulty(difficultyLevel);

        GetComponent<Button>().colors = normalColors;

        grayColors = normalColors;
        grayColors.normalColor = Color.gray; // Kolor szary
        grayColors.highlightedColor = Color.gray; // Kolor szary
        grayColors.pressedColor = Color.gray; // Kolor szary

        easyButton.colors = grayColors ;
    }
}
