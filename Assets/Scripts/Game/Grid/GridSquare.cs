using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour // klasa reprezentująca pole planszy
{
    public Image hooverImage; // image który jest wyświetlany gdy ShapeSquare wchodzi z nim w kolizję

    public Image symbolImage; // image który jest wyświetlany gdy gracz umiejscowi na tym polu symbol z kafelka


    public bool selected; // czy wchodzimy z tym kwadratem w kolizję(najeżdżamy na niego kafelkiem)
    public int squareIndex; // indeks tego pola, pozwala na identyfikację jego położenia względem innych pól planszy
    public bool squareOccupied; // zmienna mówiąca czy pole już jest zajęte

    public bool symbolPut; // zmienna określająca czy na tym polu gracz już umiejcowił symbol

    public int symbolIndex = -1; // indeks symbolu (indeks dotyczący listy dostępnych symboli) który jest na kafelku, domyślnie- brak symbolu

    private int indx = -1; // zmienna pomocnicza, potrzebna do manipulacji symbolami


    void Start() // metoda wywoływana  na początku istnienia instancji tego obiektu
    {
       selected = false;
       squareOccupied = false; 
       symbolPut = false;

       if(squareIndex ==0) // pole w lewym  górnym rogu planszy jest na samym początku rozgrywki zajęte
       {
        indx = symbolIndex;
        selected = true;
        squareOccupied = true; 
        symbolPut = true;
       }
    }


    public void PlaceSquareOnTheBoard() // metoda wywoływana gdy gracz umieści symbol na tym polu
    {
        hooverImage.gameObject.SetActive(false);       
        selected = true;
        squareOccupied = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) // metoda wywoływana gdy ShapeSquare wejdzie w kolizję z tym polem
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
    private void OnTriggerStay2D(Collider2D collision) // metoda wywoływana w trakcie kolizji 
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
            collision.GetComponent<ShapeSquare>().SetOccupied();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // metoda wywoływana na koniec kolizji, tu następuje ewentualne wyąwietlenie symbolu na tym polu oraz sprawdzenie warunku końca gry
    {
        if(squareOccupied == false)
        {
            selected = false;
            hooverImage.gameObject.SetActive(false);
        }
        if(collision.GetComponent<ShapeSquare>() != null && squareOccupied == true)
        {
            
            symbolImage.gameObject.SetActive(true);
            symbolPut = true;
            symbolIndex = indx;
            collision.GetComponent<ShapeSquare>().UnsetOccupied();

        }
        GameEvents.EndOfGame();
    }




}
