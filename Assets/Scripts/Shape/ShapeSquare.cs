using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
 public Image occupiedImage;
 public Image symbolImage;
 public List<Sprite> symbols; 

public int symbolIndex = 0; 

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

 public void DeactivateSquare()
 {
   gameObject.GetComponent<BoxCollider2D>().enabled = false;
   gameObject.SetActive(false);
 }

 public void ActivateSquare()
 {
  
    gameObject.GetComponent<BoxCollider2D>().enabled = true;
    gameObject.SetActive(true);

    
    symbolImage.gameObject.SetActive(true);

 }

 public void SetOccupied()
 {
  occupiedImage.gameObject.SetActive(true);
   symbolImage.gameObject.SetActive(true);
 }


 public void UnsetOccupied()
 {
  occupiedImage.gameObject.SetActive(false);
   symbolImage.gameObject.SetActive(true);
 }

public void ReloadSymbol()
{
    if (gameObject.activeSelf) // Upewnij się, że kwadrat jest aktywny
    {
        // Losowanie indeksu symbolu i ustawienie go w occupiedImage
        int randomIndex = Random.Range(0, symbols.Count);
        symbolImage.sprite = symbols[randomIndex];
        symbolIndex = randomIndex;
        // Pokazanie symbolImage
        symbolImage.gameObject.SetActive(true);
        //symbolImage.GetComponent<RectTransform>().Rotate(Vector3.forward, 90f);
    }
}


}
