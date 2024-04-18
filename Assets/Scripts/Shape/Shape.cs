using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale; // lezacy kafelek jest troszeczke mniejszy od podnoszonego
    public Vector2 offset = new Vector2(0f,700f);

    public int TotalSquareNumber{get;set;}

    public List<GameObject> currentShape = new List<GameObject>();
    private Vector3 shapeStartScale;
    private RectTransform transformed;
    private Canvas canvas;
    private Vector3 startPosition;

    private bool shapeActive = true;

    private Quaternion startRotation; // do zapisania rotacji

    public void Awake()
    {
        shapeStartScale = this.GetComponent<RectTransform>().localScale; // poczatkowa skala obiektu
        transformed = this.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = transformed.localPosition;
        shapeActive = true;
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

    public bool IsOnStartPosition()
    {
        return transformed.localPosition == startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach(var square in currentShape)
        {
            if (square.activeSelf)
                return true;
        }

        return false;
    }

    public void DeactivateShape()
    {
        if (shapeActive)
        {
            foreach(var square in currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateSquare();
            }
        }
        shapeActive = false;
    }

    private void SetShapeInactive()
    {
           //GameEvents.EndOfGame();
        if(IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
  
    }

    public void ActivateShape()
    {
        if (!shapeActive)
        {
            foreach(var square in currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateSquare();
            }
        }
        shapeActive = true;
    }

    public void RequestNewShape()
    {
        //GameEvents.EndOfGame();
        transformed.localPosition = startPosition;
         transformed.rotation = Quaternion.identity;
        CreateShape();
        ReloadShapeSymbols(); 

    }

    public void ReloadShapeSymbols()
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


public void CreateShape()
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

    public List<Sprite> GetShapeSymbols()
    {
        List<Sprite> symbols = new List<Sprite>();

        foreach (var square in currentShape)
        {
            if (square.activeSelf)
            {
                ShapeSquare shapeSquare = square.GetComponent<ShapeSquare>();
                if (shapeSquare != null)
                {
                    Image symbolImage = shapeSquare.symbolImage;
                    if (symbolImage != null)
                    {
                        symbols.Add(symbolImage.sprite);
                    }
                }
            }
        }

        return symbols;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RotateShape();
    }
     public void OnPointerUp(PointerEventData eventData)
     {

     }

     public void OnBeginDrag(PointerEventData eventData)
     {
        startRotation = transformed.rotation;
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
     }
      public void OnDrag(PointerEventData eventData)
      {
        // transformed.anchorMin = new Vector2(0.5f,0.5f);
        // transformed.anchorMax = new Vector2(0.5f,0.5f);
        // transformed.pivot = new Vector2(0.5f,0.5f);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        eventData.position, Camera.main, out pos);

        transformed.localPosition = pos + offset;
      }

    public void OnEndDrag(PointerEventData eventData)
    {
       transformed.rotation = startRotation;
       this.GetComponent<RectTransform>().localScale = shapeStartScale;

     foreach (var square in currentShape) // Obrót symboli tak, aby pasowały do obrotu kafelka
    {
        var symbolTransform = square.GetComponent<ShapeSquare>().symbolImage.GetComponent<RectTransform>();

        // Pobierz kąt rotacji kafelka wzdłuż osi Z
        float tileZRotation = startRotation.eulerAngles.z;

        // Oblicz nowy kąt rotacji symbolu, przeciwny do rotacji kafelka wzdłuż osi Z
        float symbolZRotation = -tileZRotation;

        // Ustaw rotację symbolu
        symbolTransform.localRotation = Quaternion.Euler(0f, 0f, symbolZRotation);
    }
       GameEvents.CheckIfShapeCanBePlaced();
     
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    
    }


    private void MoveShapeToStartPosition()
    {
            transformed.transform.localPosition = startPosition;
    }

    private void RotateShape()
    {
        // Obracaj kształt o 90 stopni w prawo
        transformed.Rotate(Vector3.forward, -90f); //obrót kafelka

          foreach (var square in currentShape) // obrót symboli
            {
                square.GetComponent<ShapeSquare>().symbolImage.GetComponent<RectTransform>().Rotate(Vector3.forward, 90f);
            }
    }
}
