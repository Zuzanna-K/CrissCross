using UnityEngine;

public class ShapeStorage : MonoBehaviour // klasa ułatwiająca dostęp do kafelka aktywnego w grze
{
    public Shape currentShape; // aktualny kafelek w grze

    private void OnEnable()
    {
        GameEvents.RequestNewShape += RequestNewShape;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShape -= RequestNewShape;
    }

    void Start() // tworzenie pierwszego kafelka w grze
    {
      currentShape.CreateShape();
    }


    public Shape GetCurrentSelectedShape() // zwraca obecny w grze kafelek
    {
         if (!currentShape.IsOnStartPosition() && currentShape.IsAnyOfShapeSquareActive())
                return currentShape;

                else return null;
    }

    private void RequestNewShape() // ładuje nowy kafelek
    {
        currentShape.RequestNewShape();
    }


}
