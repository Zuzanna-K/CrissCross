using UnityEngine;
using UnityEngine.UI;

public class EasyButton : MonoBehaviour // skrypt odpowiedzialny za wybranie łatwego poziomu trudnośći
{
    public int difficultyLevel;
    public Button difficultButton; // wskazanie na przycisk trudnego poziomu trudności

    private ColorBlock normalColors;

    private ColorBlock grayColors; 

    private void Start() 
    {
        difficultyLevel = 0;
        
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        normalColors = button.colors;

        grayColors = normalColors;
        grayColors.normalColor = Color.gray;
        grayColors.highlightedColor = Color.gray;
        grayColors.pressedColor = Color.gray; 

        if (GameManager.instance.selectedDifficulty == 1)
        {
            button.colors = grayColors; 
        }
        else
        {
            button.colors = normalColors;
        }
    }

    public void OnClick() // funkcja wywoływana po kliknięciu na przycisk
    {
        GameManager.instance.ChangeDifficulty(difficultyLevel); // zmiana poziomu trudności
        ColorBlock colors = normalColors;
        GetComponent<Button>().colors = colors;
        
        difficultButton.colors = grayColors ;
    }
}
