using UnityEngine;

public class ExitGame : MonoBehaviour // skrypt pozwalający na wyjście z aplikacji, przypisany do przyciku ExitBtn
{
    public void QuitGame()
    {
        Application.Quit();
    }
}
