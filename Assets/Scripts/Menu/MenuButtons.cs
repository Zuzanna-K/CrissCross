using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour // skrypt pozwalający nawigować między scenami MainMenu(Menu Główne) i Game (Gra)
{
    private void Awake()
    {
        if(Application.isEditor == false)
            Debug.unityLogger.logEnabled = false;
    }

    public void LoadScene(string name) // załadowanie wskazanej sceny
    {
        SceneManager.LoadScene(name);
    }
}
