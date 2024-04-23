using UnityEngine;
using UnityEngine.UI;

public class DifficultButton : MonoBehaviour // skrypt odpowiedzialny za wybranie trudnego poziomu trudności
{
    public int difficultyLevel;
    public Button easyButton; // wskazanie na przycisk łatwego poziomu trudności

     private ColorBlock normalColors;
     private ColorBlock grayColors; 


    private void Start()
    {
       
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        normalColors = button.colors;

        grayColors = normalColors;
        grayColors.normalColor = Color.gray; 
        grayColors.highlightedColor = Color.gray; 
        grayColors.pressedColor = Color.gray;

        if (GameManager.instance.selectedDifficulty == 0)
        {
            button.colors = grayColors; 
        }
        else
        {
            button.colors = normalColors;
        }
    }

    public void OnClick() // funkcja wywoływana po naciśnięciu na przycisk
    {
        difficultyLevel = 1;
        GameManager.instance.ChangeDifficulty(difficultyLevel); // zmiana poziomu trudności

        GetComponent<Button>().colors = normalColors;

        easyButton.colors = grayColors ;
    }
}
