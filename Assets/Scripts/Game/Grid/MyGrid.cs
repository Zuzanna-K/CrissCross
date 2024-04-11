using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;

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

    public Text finalScore;
    public List <Text> rowsColsScores = new();
    

    private Vector2 offset =new(0.0f,0.0f);
    private List<GameObject> gridSquares = new();

    private List<int>scoresRowsColumns = new();

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.EndOfGame += EndOfGame;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.EndOfGame -= EndOfGame;
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
    var symbolsToAdd = new List<Sprite>(); // Lista symboli do dodania na planszę

    foreach (var square in gridSquares)
    {
        var gridSquare = square.GetComponent<GridSquare>();

        if (gridSquare.selected && !gridSquare.squareOccupied)
        {
            squareIndexes.Add(gridSquare.squareIndex);
            gridSquare.selected = false;
            // gridSquare.ActivateSquare();
        }
    }

    var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
    if (currentSelectedShape == null) return;

    if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)
    {
        // Przypisanie symboli z kształtu na planszę
        for (int i = 0; i < squareIndexes.Count; i++)
        {
            var gridSquare = gridSquares[squareIndexes[i]].GetComponent<GridSquare>();
            gridSquare.PlaceSquareOnTheBoard();
            //symbolsToAdd.Add(currentSelectedShape.currentShape[i].GetComponent<ShapeSquare>().symbolImage.sprite);
           // gridSquare.SetSymbol(gridSquare.GetCollidingShapeSquare().symbolImage.sprite);
            
           // gridSquare.SetSymbol(currentSelectedShape.currentShape[i].GetComponent<ShapeSquare>().symbolImage.sprite);

        }


    //     var shapeLeft = 0;

    //     foreach (var shape in shapeStorage.shapeList)
    //     {
    //         if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
    //         {
    //             shapeLeft++;
    //         }
    //     }
        
    //     // if (shapeLeft == 0 && ConditionToEnd())
    //     // {
    //     //     EndOfGame();
    //     // }
        
    //     if (shapeLeft == 0)
    //     {
    //         GameEvents.RequestNewShape();
    //     }
    //     else
    //     {
             GameEvents.SetShapeInactive();

             if(!ConditionToEnd())
             {
                LoadNewShape();
             }
    //     }
     }
    else // cant place shape on the board
    {
        GameEvents.MoveShapeToStartPosition();
    }

   // LoadNewShape();
}




private void LoadNewShape()
{
    
        var shapeLeft = 0;

        foreach (var shape in shapeStorage.shapeList)
        {
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                shapeLeft++;
            }
        }
        
        if (shapeLeft == 0)
        {
            GameEvents.RequestNewShape();
        }
 
    }




private void EndOfGame()
{
    if (ConditionToEnd() == true)
    {

        //SceneManager.LoadScene("Game");
        finalScore.text = "    Final Score: "+ scoring().ToString();
        //DisplayRowColScores();
    }
    else return;
}

private bool ConditionToEnd()
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < columns; j++)
        {
            var currentSquare = gridSquares[i * columns + j].GetComponent<GridSquare>();

            if (!currentSquare.symbolPut)
            {
                if (j > 0 && !gridSquares[i * columns + j - 1].GetComponent<GridSquare>().symbolPut)
                    return false;
                
                if (j < columns - 1 && !gridSquares[i * columns + j + 1].GetComponent<GridSquare>().symbolPut)
                    return false;

                if (i > 0 && !gridSquares[(i - 1) * columns + j].GetComponent<GridSquare>().symbolPut)
                    return false;

                if (i < rows - 1 && !gridSquares[(i + 1) * columns + j].GetComponent<GridSquare>().symbolPut)
                    return false;
            }
        }
    }

    return true; 
}

private int scoring()
{
    int totalScore = 0;

    // Sprawdź rząd
    for (int i = 0; i < rows; i++)
    {
        int rowScore = 0;
        int consecutiveCount = 1;
        int previousSymbolIndex = -2;

        for (int j = 0; j < columns; j++)
        {
            var currentSquare = gridSquares[i * columns + j].GetComponent<GridSquare>();

            if (currentSquare.symbolPut)
            {
                if (currentSquare.symbolIndex == previousSymbolIndex)
                {
                    consecutiveCount++;
                }
                else
                {
                    if (consecutiveCount >= 2)
                    {
                        rowScore += GetScoreFromConsecutiveCount(consecutiveCount);
                    }
                    consecutiveCount = 1;
                }

                previousSymbolIndex = currentSquare.symbolIndex;
            }
            
            else // kafelek bez symbolu
            {
                previousSymbolIndex = -2;
                consecutiveCount = 1;
            }
        }

        if (consecutiveCount >= 2)
        {
            rowScore += GetScoreFromConsecutiveCount(consecutiveCount);
        }

        

        
        totalScore += rowScore;
        rowsColsScores[i].text = rowScore.ToString();
        //scoresRowsColumns.Add(rowScore);
    }

    // Sprawdź kolumnę
    for (int j = 0; j < columns; j++)
    {
        int columnScore = 0;
        int consecutiveCount = 1;
        int previousSymbolIndex = -2;

        for (int i = 0; i < rows; i++)
        {
            var currentSquare = gridSquares[i * columns + j].GetComponent<GridSquare>();

            if (currentSquare.symbolPut)
            {
                if (currentSquare.symbolIndex == previousSymbolIndex)
                {
                    consecutiveCount++;
                }
                else
                {
                    if (consecutiveCount >= 2)
                    {
                        columnScore += GetScoreFromConsecutiveCount(consecutiveCount);
                    }
                    consecutiveCount = 1;
                }

                previousSymbolIndex = currentSquare.symbolIndex;
            }
            else // kafelek bez symbolu
            {
                previousSymbolIndex = -2;
                consecutiveCount = 1;
            }
        }

        if (consecutiveCount >= 2)
        {
            columnScore += GetScoreFromConsecutiveCount(consecutiveCount);
        }
       
        totalScore += columnScore;
        rowsColsScores[rows + j].text = columnScore.ToString();
        //scoresRowsColumns.Add(columnScore);
    }

    return totalScore;
}

private int GetScoreFromConsecutiveCount(int count)
{
    if (count == 2)
    {
        return 2;
    }
    else if (count == 3)
    {
        return 3;
    }
    else if (count == 4)
    {
        return 8;
    }
    else if (count >= 5)
    {
        return 10;
    }
    else
    {
        return 0; // niepoprawna liczba
    }
}

private void DisplayRowColScores()
{
    for(int i = 0 ; i < rowsColsScores.Count ; i++)
    {
        rowsColsScores[i].text =scoresRowsColumns[i].ToString();
    }
}


}
