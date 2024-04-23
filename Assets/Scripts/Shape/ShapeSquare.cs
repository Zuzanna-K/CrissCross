using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour // klasa reprezentująca pojedynczy kwadrat budujący kafelek
{
 public Image occupiedImage; // image wyświetlający się gdy ten kwadrat wchodzi w kolizję z zajętym polem planszy
 public Image symbolImage; // image przedstawiający symbol na kwadracie kafelka
 public List<Sprite> symbols; // lista symboli, z których zostanie wylosowany symbol znajdujący się na kafelku

public int symbolIndex = 0; // indeks wylosowanego sybolu, ułatwia porównywanie symboli na kafelkach przy zliczaniu punktacji końcowej

 void Start()
 {
    occupiedImage.gameObject.SetActive(false);

    // Losowanie indeksu symbolu i ustawienie go w occupiedImage
    int randomIndex = Random.Range(0, symbols.Count);
    symbolImage.sprite = symbols[randomIndex];
    symbolIndex = randomIndex;

    // Pokazanie occupiedImage
    symbolImage.gameObject.SetActive(true);
 }


 public void SetOccupied() // metoda wywoływana gdy kwadrat wejdzie w kolizję z zajętym polem, podświetla kwadrat na czerwono
 {
  occupiedImage.gameObject.SetActive(true);
  symbolImage.gameObject.SetActive(true);
 }


 public void UnsetOccupied() // metoda wywoływana gdy kolizja z zajętym polem się zakończy
 {
  occupiedImage.gameObject.SetActive(false);
  symbolImage.gameObject.SetActive(true);
 }

public void ReloadSymbol() // lowowanie nowego symbolu
{
    if (gameObject.activeSelf) 
    {
        // Losowanie indeksu symbolu i ustawienie go w occupiedImage
        int randomIndex = Random.Range(0, symbols.Count);
        symbolImage.sprite = symbols[randomIndex];
        symbolIndex = randomIndex;
        // Pokazanie symbolImage
        symbolImage.gameObject.SetActive(true);
    }
}


}
