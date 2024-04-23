using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyGrid : MonoBehaviour
{
    public ShapeStorage shapeStorage;

    public GameObject gridSquare;
    public Vector2 startPosition = new (0.0f,0.0f);
    public float squareScale = 0.5f;

     public List<Sprite> symbols; 

    public Text finalScore;
    public List <Text> rowsColsScores = new();
    

    private Vector2 offset =new(0.0f,0.0f);
    private List<GameObject> gridSquares = new();
    private int columns = 5;
    private int rows = 5;

    private void OnEnable() // subskrybcja metod, pozwala na "komunikację" z innymi obiektami gry
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

    public void CreateGrid() // metoda tworząca planszę - tworzy jej pola i je umiejscawia za pomocą 2 metod
    {
        SpawnGridSquares();
        SetGridSquaresPositions();

    }

    private void SpawnGridSquares() // metoda tworząca pola planszy
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

                squareIndex++;
            }
        }
    }

    private void SetGridSquaresPositions() // metoda umieszczająca pola planszy
    {
        int columnNumber = 0; 
        int rowNumber = 0; 

        var squareRect = gridSquares[0].GetComponent<RectTransform>();

        offset.x = squareRect.rect.width * squareRect.transform.localScale.x ;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y;

        foreach(GameObject square in gridSquares)
        {
            if(rowNumber == 0 && columnNumber == 0)
            {
                var gridSquare = square.GetComponent<GridSquare>();
                    // Losowanie indeksu symbolu i ustawienie go w occupiedImage
                int randomIndex = Random.Range(0, symbols.Count);
                gridSquare.symbolImage.sprite = symbols[randomIndex];
                gridSquare.symbolIndex = randomIndex;
                gridSquare.symbolImage.gameObject.SetActive(true);
            }
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

private void CheckIfShapeCanBePlaced() // metoda sprawdzająca czy gracz może położyć kafelek w wybranym przez siebie miejscu
{   
    var squareIndexes = new List<int>();

    foreach (var square in gridSquares)
    {
        var gridSquare = square.GetComponent<GridSquare>();

        if (gridSquare.selected && !gridSquare.squareOccupied)
        {
            squareIndexes.Add(gridSquare.squareIndex);
            gridSquare.selected = false;
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
        }

 
             GameEvents.SetShapeInactive();

             if(!ConditionToEnd())
             {
                LoadNewShape();
             }
  
     }
    else // cant place shape on the board
    {
        GameEvents.MoveShapeToStartPosition();
    }

}




private void LoadNewShape() // metoda tworząca nowy kafelek, który gracz musi dopasować
{
    
    if(!shapeStorage.currentShape.IsOnStartPosition()&& !shapeStorage.currentShape.IsAnyOfShapeSquareActive())    
        {
            GameEvents.RequestNewShape();
        }
 
}




private void EndOfGame() // metoda wywoływana na koniec bieżącej rozgrywki, wyświetlająca punktację końcową
{
    if (ConditionToEnd() == true)
    {
        finalScore.text = "Wynik: "+ Scoring().ToString();
    }
    else return;
}

private bool ConditionToEnd() // metoda sprawdzająca czy bieżąca rozgrywka powinna się zakończyć
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

private int Scoring() // metoda licząca punktację na podstawie aktualnego stanu planszy
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
                if (consecutiveCount >= 2)
                {
                   rowScore += GetScoreFromConsecutiveCount(consecutiveCount);
                  Debug.Log("koniec liczenia: w kolumnie: " + j + " w wierszu: " + i + " symboli z rzędu: " + consecutiveCount);
                }
                previousSymbolIndex = -2;
                consecutiveCount = 1;
            }
        }

        if (consecutiveCount >= 2)
        {
            rowScore += GetScoreFromConsecutiveCount(consecutiveCount);
        }

        

        if(GameManager.instance.selectedDifficulty == 1 && rowScore == 0) // punktacja dla trybu zaawansowanego
        {
            rowScore = -5;
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
                    //Debug.Log("symbol z rzędu: "+consecutiveCount + " w kolumnie: " + j + " w wierszu: " + i);
                }
                else
                {
                    if (consecutiveCount >= 2)
                    {
                        columnScore += GetScoreFromConsecutiveCount(consecutiveCount);
                        Debug.Log("koniec liczenia: w kolumnie: " + j + " w wierszu: " + i + " symboli z rzędu: " + consecutiveCount);
                    }
                    consecutiveCount = 1;
                }

                previousSymbolIndex = currentSquare.symbolIndex;
            }
            else // kafelek bez symbolu
            {
              if (consecutiveCount >= 2)
                {
                   columnScore += GetScoreFromConsecutiveCount(consecutiveCount);
                  Debug.Log("koniec liczenia: w kolumnie: " + j + " w wierszu: " + i + " symboli z rzędu: " + consecutiveCount);
                }
                previousSymbolIndex = -2;
                consecutiveCount = 1;
            }
        }

        if (consecutiveCount >= 2)
        {
            columnScore += GetScoreFromConsecutiveCount(consecutiveCount);
            Debug.Log("Drugi - ewentualny koniec liczenia: w kolumnie: " + j  + " symboli z rzędu: " + consecutiveCount);
        }
       
        if(GameManager.instance.selectedDifficulty == 1 && columnScore == 0) // punktacja dla trybu zaawansowanego
        {
            columnScore = -5;
        }
        totalScore += columnScore;
        rowsColsScores[rows + j].text = columnScore.ToString();
        //scoresRowsColumns.Add(columnScore);
    }

    if (GameManager.instance.selectedDifficulty == 1) // punkty przekatnej prawy gorny rog - lewy dolny rog
    {
        int diameterScore = 0;
        int consecutiveCount = 1;
        int previousSymbolIndex = -2;
 
        for (int i = 0; i < rows; i++)
        {
         
            var currentSquare = gridSquares[(i+1)*(rows -1)].GetComponent<GridSquare>();

            if (currentSquare.symbolPut)
            {
                if (currentSquare.symbolIndex == previousSymbolIndex)
                {
                    consecutiveCount++;
                    //Debug.Log("symbol z rzędu: "+consecutiveCount + " w kolumnie: " + j + " w wierszu: " + i);
                }
                else
                {
                    if (consecutiveCount >= 2)
                    {
                        diameterScore += GetScoreFromConsecutiveCount(consecutiveCount);
                        Debug.Log("koniec liczenia: po przekatnej w wierszu: " + i + " symboli z rzędu: " + consecutiveCount);
                    }
                    consecutiveCount = 1;
                }

                previousSymbolIndex = currentSquare.symbolIndex;
            }

            else // kafelek bez symbolu
            {
              if (consecutiveCount >= 2)
                {
                  diameterScore += GetScoreFromConsecutiveCount(consecutiveCount);
                  Debug.Log("koniec liczenia: po przekatnej w wierszu: " + i + " symboli z rzędu: " + consecutiveCount);
                }
                previousSymbolIndex = -2;
                consecutiveCount = 1;
            }

        }

        if (consecutiveCount >= 2)
        {
            diameterScore += GetScoreFromConsecutiveCount(consecutiveCount);
            Debug.Log("Drugi - ewentualny koniec liczenia po przekatnej symboli z rzędu: " + consecutiveCount);
        }

        if(diameterScore == 0)
        {
            diameterScore = -5;
        }


        rowsColsScores[rows + columns].text = diameterScore.ToString();
        rowsColsScores[rows + columns +1].text = diameterScore.ToString();
        totalScore += 2*diameterScore;
    }

    return totalScore;
}

private int GetScoreFromConsecutiveCount(int count) // metoda licząca punkty za jeden zestaw symboli
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
        return 0; 
    }
}


}
