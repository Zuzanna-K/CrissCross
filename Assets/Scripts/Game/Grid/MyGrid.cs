using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class MyGrid : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.0f;
    public GameObject gridSquare;
    public Vector2 startPosition = new (0.0f,0.0f);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0.0f;

    private Vector2 offset =new(0.0f,0.0f);
    private List<GameObject> gridSquares = new();

    
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();

    }

    private void SpawnGridSquares()
    {
        int squareIndex = 0;

        for (var row =0 ; row < rows ; ++row)
        {
            for (var column =0; column < columns ; ++column)
            {
                gridSquares.Add(Instantiate(gridSquare) as GameObject);

                gridSquares[gridSquares.Count-1].GetComponent<GridSquare>().squareIndex = squareIndex;
                gridSquares[gridSquares.Count-1].transform.SetParent(this.transform); // kafelek zaczyna być dzieckiem obiektu do którego przypiszemy ten skrypt
                gridSquares[gridSquares.Count-1].transform.localScale = new Vector3(squareScale,squareScale,squareScale);

                gridSquares[gridSquares.Count-1].GetComponent<GridSquare>().SetImage(squareIndex%2 == 0);
                squareIndex++;
            }
        }
    }

    private void SetGridSquaresPositions()
    {
        int columnNumber = 0; //numer kolumny w ktoreym sie znajdujemy
        int rowNumber = 0; // nr wiersza 

        var squareRect = gridSquares[0].GetComponent<RectTransform>();

        offset.x = squareRect.rect.width * squareRect.transform.localScale.x + everySquareOffset;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y + everySquareOffset;

        foreach(GameObject square in gridSquares)
        {
            if (columnNumber +1 > columns)
            {
                columnNumber = 0;
                rowNumber++;
            }

            var positionXOffset = offset.x * columnNumber ;
            var positionYOffset = offset.y * rowNumber;


            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + positionXOffset, 
            startPosition.y - positionYOffset);

            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + positionXOffset, 
            startPosition.y - positionYOffset, 0.0f);

            columnNumber++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {

        var squareIndexes = new List<int>();
        foreach(var square in gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if(gridSquare.selected && !gridSquare.squareOccupied)
            {
                squareIndexes.Add(gridSquare.squareIndex);
                gridSquare.selected = false;
               // gridSquare.ActivateSquare();
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if(currentSelectedShape == null) return;

        if(currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {
             // Zbieramy symbole z aktualnego kształtu

        List<Sprite> symbols = new List<Sprite>();

        foreach (var square in currentSelectedShape.currentShape)
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
        symbols = currentSelectedShape.GetShapeSymbols();

        // Przypisanie symboli na planszy
        for (int i = 0; i < squareIndexes.Count; i++)
        {
            var gridSquare = gridSquares[squareIndexes[i]].GetComponent<GridSquare>();
            gridSquare.PlaceSquareOnTheBoard();
            gridSquare.SetSymbol(symbols[i]);
            
        }


            var shapeLeft = 0;

            foreach(var shape in shapeStorage.shapeList)
            {
                if(shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }
            

            if(shapeLeft == 0)
            {
                GameEvents.RequestNewShape();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

   
        }
        else // cant place shape on the board
        {
            GameEvents.MoveShapeToStartPosition();
        }

    }
}
