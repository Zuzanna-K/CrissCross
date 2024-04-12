using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale; // lezacy kafelek jest troszeczke mniejszy od podnoszonego
    public Vector2 offset = new Vector2(0f,700f);


    [HideInInspector]
    public ShapeData currentShapeData;

    public int TotalSquareNumber{get;set;}

    public List<GameObject> currentShape = new List<GameObject>();
    private Vector3 shapeStartScale;
    private RectTransform transformed;
    private bool shapeDraggable = true;
    private Canvas canvas;
    private Vector3 startPosition;

    private bool shapeActive = true;

    private Quaternion startRotation; // do zapisania rotacji

    public void Awake()
    {
        shapeStartScale = this.GetComponent<RectTransform>().localScale; // poczatkowa skala obiektu
        transformed = this.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        shapeDraggable = true;
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

    public void RequestNewShape(ShapeData shapeData)
    {
        //GameEvents.EndOfGame();
        transformed.localPosition = startPosition;
         transformed.rotation = Quaternion.identity;
        CreateShape(shapeData);
        ReloadShapeSymbols(); // Dodaj to, aby ponownie losować symbole dla nowego kształtu

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


public void CreateShape(ShapeData shapeData)
{

    currentShapeData = shapeData;
    TotalSquareNumber = GetNumberOfSquares(shapeData);

    while (currentShape.Count < TotalSquareNumber)
    {
        GameObject newSquare = Instantiate(squareShapeImage, transform) as GameObject;
        newSquare.gameObject.transform.position = Vector3.zero;
        newSquare.gameObject.SetActive(false);

        currentShape.Add(newSquare);
    }

    var squareRect = squareShapeImage.GetComponent<RectTransform>();
    var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
        squareRect.rect.height * squareRect.localScale.y);

    int currentIndexInist = 0;

    for (var row = 0; row < shapeData.rows; row++)
    {
        for (var column = 0; column < shapeData.columns; column++)
        {
            if (shapeData.board[row].column[column]) // aktywny kwadracik
            {
                currentShape[currentIndexInist].SetActive(true);
                currentShape[currentIndexInist].GetComponent<RectTransform>().localPosition =
                    new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                        GetYPositionForShapeSquare(shapeData, row, moveDistance));

                // Losowanie indeksu symbolu i ustawienie go w occupiedImage
                ShapeSquare shapeSquare = currentShape[currentIndexInist].GetComponent<ShapeSquare>();
                int randomIndex = Random.Range(0, shapeSquare.symbols.Count);
                shapeSquare.symbolImage.sprite = shapeSquare.symbols[randomIndex];

                currentIndexInist++;
            }
        }
    }
}


    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

         if (shapeData.rows > 1)
        {
            if(shapeData.rows % 2 != 0) 
            {
                var middleSquareIndex = (shapeData.rows-1)/2;
                var multiplier = (shapeData.rows-1)/2;

                if (row < middleSquareIndex)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY*= multiplier;
                }
                else if (row>middleSquareIndex)
                {
                     shiftOnY = moveDistance.y * -1;
                     shiftOnY *= multiplier;
                }
            }

            else
            {
                var middleSquareIndex2 = (shapeData.rows ==2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows ==2) ? 0 : shapeData.rows - 1;
                var multiplier = shapeData.rows /2;

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if(row == middleSquareIndex1)
                        shiftOnY = moveDistance.y / 2;
                    if (row == middleSquareIndex2)
                        shiftOnY = (moveDistance.y / 2) * -1;
                }

                if(row < middleSquareIndex1 && row < middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if(row > middleSquareIndex1 && row > middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;

                }
            }

        }

        return shiftOnY;

        
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {

        float shiftOnX = 0f;

        if (shapeData.columns > 1)
        {
            if(shapeData.columns % 2 != 0) // nieparzysta ilosc kolumn
            {
                var middleSquareIndex = (shapeData.columns-1)/2;
                var multiplier = (shapeData.columns-1)/2;

                if (column<middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX*= multiplier;
                }
                else if (column>middleSquareIndex)
                {
                     shiftOnX = moveDistance.x * 1;
                     shiftOnX *= multiplier;
                }
            }

            else
            {
                var middleSquareIndex2 = (shapeData.columns ==2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns ==2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns /2;

                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if(column == middleSquareIndex2)
                        shiftOnX = moveDistance.x / 2;
                    if (column == middleSquareIndex1)
                        shiftOnX = (moveDistance.x / 2) * -1;
                }

                if(column < middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if(column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;

                }
            }

        }

        return shiftOnX;

    }

    private int GetNumberOfSquares(ShapeData shapeData) // zwroci ilosc aktywnych kwadratów z kształtu
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach(var active in rowData.column)
            {
                if(active)
                {
                    number++;
                }
            }
        }

        return number;
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
    
        //MOJE
        // transformed.pivot = new Vector2(0.5f, 0.5f);
        // transformed.anchorMin = new Vector2(0.5f, 0.5f);
        // transformed.anchorMax = new Vector2(0.5f, 0.5f);
        // KONIEC MOJEGO

            transformed.transform.localPosition = startPosition;
        //transformed.transform.localScale = shapeStartScale;
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
