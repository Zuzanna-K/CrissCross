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

    public Image symbolImage;

    public List<Sprite> normalImages;

    public bool selected; // czy wchodzimy z tym kwadratem w kolizję(najeżdżamy na niego kafelkiem)
    public int squareIndex;
    public bool squareOccupied; // czy już jest zajęty przez poprzednie kafelki

    public bool symbolPut;

    public int symbolIndex = -1;

    private int indx = -1;

    public ShapeSquare collidingShapeSquare;

    public ShapeSquare GetCollidingShapeSquare()
    {
        return collidingShapeSquare;
    }

    void Start()
    {
       selected = false;
       squareOccupied = false; 
       symbolPut = false;
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
        //symbolImage.gameObject.SetActive(true);

        selected = true;
        squareOccupied = true;

    }

    public void SetSymbol(Sprite symbolSprite)
    {
        //symbolImage.sprite = collidingShapeSquare.symbolImage.sprite;
        symbolImage.sprite = symbolSprite;
        symbolImage.gameObject.SetActive(true);
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
            collidingShapeSquare = collision.GetComponent<ShapeSquare>();
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        selected = true;
        if(squareOccupied == false)
        {
            hooverImage.gameObject.SetActive(true);
            Image symbol = collision.GetComponent<ShapeSquare>().symbolImage;
            symbolImage.sprite = symbol.sprite;
            indx = collision.GetComponent<ShapeSquare>().symbolIndex;
        }
        else if(collision.GetComponent<ShapeSquare>() != null)
        {
          //  collidingShapeSquare = collision.GetComponent<ShapeSquare>();
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
        if(collision.GetComponent<ShapeSquare>() != null && squareOccupied == true)
        {
            // Przepisanie symbolu

            symbolImage.gameObject.SetActive(true);
            symbolPut = true;
            symbolIndex = indx;
          // collidingShapeSquare = collision.GetComponent<ShapeSquare>();
            collision.GetComponent<ShapeSquare>().UnsetOccupied();

        }
        GameEvents.EndOfGame();
    }




}
