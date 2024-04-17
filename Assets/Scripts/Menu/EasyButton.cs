using UnityEngine;
using UnityEngine.UI;

public class EasyButton : MonoBehaviour
{
    public int difficultyLevel;
    public Button difficultButton;

    private ColorBlock normalColors;

    private ColorBlock grayColors; 

    private void Start()
    {
        // Domyślnie ustawiaj łatwy poziom trudności jako wybrany
        difficultyLevel = 0;
        
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        normalColors = button.colors;
    }

    public void OnClick()
    {
        // Zapisz wybór trudności w zmiennej globalnej (np. w GameManagerze)
        GameManager.instance.ChangeDifficulty(difficultyLevel);

                // Zmieniaj kolory przycisku w zależności od jego stanu
        ColorBlock colors = normalColors;
        GetComponent<Button>().colors = colors;
        

        grayColors = normalColors;
        grayColors.normalColor = Color.gray; // Kolor szary
        grayColors.highlightedColor = Color.gray; // Kolor szary
        grayColors.pressedColor = Color.gray; // Kolor szary

        difficultButton.colors = grayColors ;
    }
}
