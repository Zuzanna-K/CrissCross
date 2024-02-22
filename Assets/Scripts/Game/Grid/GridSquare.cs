using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hooverImage;
    public Image activeImage;

    public Image normalImage;
    public List<Sprite> normalImages;

    public bool selected; // czy wchodzimy z tym kwadratem w kolizję(najeżdżamy na niego kafelkiem)
    public int squareIndex;
    public bool squareOccupied; // czy już jest zajęty przez poprzednie kafelki

    void Start()
    {
       selected = false;
       squareOccupied = false; 
    }
//temp
    public bool CanWeUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;
    }

    public void PlaceSquareOnTheBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);

        selected = true;
        squareOccupied = true;

    }

    public void SetImage(bool setFirstImage)
    {
        normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(squareOccupied == false)
        {
            selected = true;
            hooverImage.gameObject.SetActive(true);
            
        }
        else if(collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        selected = true;
        if(squareOccupied == false)
        {
            hooverImage.gameObject.SetActive(true);
        }
        else if(collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(squareOccupied == false)
        {
            selected = false;
            hooverImage.gameObject.SetActive(false);
        }
        else if(collision.GetComponent<ShapeSquare>() != null)
        {
            collision.GetComponent<ShapeSquare>().UnsetOccupied();
        }
    }




}
