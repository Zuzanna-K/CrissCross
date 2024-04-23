using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler // klasa reprezentująca kafelek
{
    public GameObject squareShapeImage; // prefabrykat pojedynczego kwadratu
    public Vector3 shapeSelectedScale; // skala podnoszonego kafelka (lezacy kafelek jest troszeczke mniejszy od podnoszonego)
    public Vector2 offset = new Vector2(0f,700f);

    public int TotalSquareNumber{get;set;} // ilość kwadratów budujących kafelek

    public List<GameObject> currentShape = new List<GameObject>(); // lista kwadratów ShapeSquare budujących ten kafelek
    private Vector3 shapeStartScale;
    private RectTransform transformed;
    private Canvas canvas; // odwołanie do canvasu, w którym znajduje się kafelek
    private Vector3 startPosition;

    private Quaternion startRotation; // do zapisania rotacji

    public void Awake()
    {
        shapeStartScale = this.GetComponent<RectTransform>().localScale; // poczatkowa skala obiektu
        transformed = this.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = transformed.localPosition;
    }

    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }
    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }

    public bool IsOnStartPosition() // sprawdza, czy kafelek znajduje się na początkowej pozycji
    {
        return transformed.localPosition == startPosition;
    }

    public bool IsAnyOfShapeSquareActive() // metoda sprawdzająca czy kafelek składa się z aktywnych kwadratów
    {
        foreach(var square in currentShape)
        {
            if (square.activeSelf)
                return true;
        }

        return false;
    }



    private void SetShapeInactive() // metoda dezaktyująca aktualny kafelek
    {
        if(IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
  
    }

    public void RequestNewShape() // metoda wywoływana, aby stworzyć nowy kafelek z nowymi symbolami
    {
        transformed.localPosition = startPosition;
        transformed.rotation = Quaternion.identity;
        CreateShape();
        ReloadShapeSymbols(); 

    }

    public void ReloadShapeSymbols() // metoda losująca nowe symbole kafelka, dostosowuje też obrót symboli
    {
        foreach (var square in currentShape)
        {
            if (square.activeSelf)
            {
                ShapeSquare shapeSquare = square.GetComponent<ShapeSquare>();
                if (shapeSquare != null)
                {
                    shapeSquare.ReloadSymbol(); // Nowa funkcja do ponownego losowania symbolu
                }
            }
        }

            foreach (var square in currentShape) // Obrót symboli tak, aby pasowały do obrotu kafelka
                {
                    var symbolTransform = square.GetComponent<ShapeSquare>().symbolImage.GetComponent<RectTransform>();
                  // Ustaw rotację symbolu
                    symbolTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
    }


public void CreateShape() // metoda tworząca kafelek - prawy i lewy SquareShape budujące go, losuje nowe symbole
{

    currentShape.Clear(); 
//lewy
    GameObject leftSquare = Instantiate(squareShapeImage, transform);
    leftSquare.SetActive(true);
    leftSquare.GetComponent<RectTransform>().localPosition = new Vector2(-squareShapeImage.GetComponent<RectTransform>().rect.width/2, 0);
    currentShape.Add(leftSquare);

 //prawy
    GameObject rightSquare = Instantiate(squareShapeImage, transform);
    rightSquare.SetActive(true);
    rightSquare.GetComponent<RectTransform>().localPosition = new Vector2(squareShapeImage.GetComponent<RectTransform>().rect.width/2, 0);
    currentShape.Add(rightSquare);

    TotalSquareNumber = currentShape.Count;
    ReloadShapeSymbols();
}

    public void OnPointerClick(PointerEventData eventData) // metoda wywoływana po kliknięciu na kafelek, obracająca go
    {
        RotateShape();
    }

     public void OnBeginDrag(PointerEventData eventData) // metoda wywoływana na początku przeciągania kafelka
     {
        startRotation = transformed.rotation;
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
     }
      public void OnDrag(PointerEventData eventData) // metoda wywoływana podczas przeciągania kafelka, aktualizująca jego pozycję
      {

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        eventData.position, Camera.main, out pos);

        transformed.localPosition = pos + offset;
      }

    public void OnEndDrag(PointerEventData eventData)// metoda wywoływana po przeciągnięciu kafelka
    {
       transformed.rotation = startRotation;
       this.GetComponent<RectTransform>().localScale = shapeStartScale;

        foreach (var square in currentShape) // Obrót symboli tak, aby pasowały do obrotu kafelka
        {
            var symbolTransform = square.GetComponent<ShapeSquare>().symbolImage.GetComponent<RectTransform>();
            float tileZRotation = startRotation.eulerAngles.z;
            float symbolZRotation = -tileZRotation;
            symbolTransform.localRotation = Quaternion.Euler(0f, 0f, symbolZRotation);
        }
        GameEvents.CheckIfShapeCanBePlaced();
     
    }

    private void MoveShapeToStartPosition() // metoda umieszczająca kafelek na jego pozycję początkową
    {
        transformed.transform.localPosition = startPosition;
    }

    private void RotateShape() // metoda pozwalająca na obrót kafelka
    {
        transformed.Rotate(Vector3.forward, -90f); //obrót kafelka

          foreach (var square in currentShape) // obrót symboli
            {
                square.GetComponent<ShapeSquare>().symbolImage.GetComponent<RectTransform>().Rotate(Vector3.forward, 90f);
            }
    }
}
