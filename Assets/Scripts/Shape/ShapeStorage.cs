using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
       public Shape currentShape;

    private void OnEnable()
    {
        GameEvents.RequestNewShape += RequestNewShape;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShape -= RequestNewShape;
    }

    void Start()
    {

      //  currentShape.CreateShape(shapeData[0]);

      currentShape.CreateShape();
    }


    public Shape GetCurrentSelectedShape()
    {
         if (!currentShape.IsOnStartPosition() && currentShape.IsAnyOfShapeSquareActive())
                return currentShape;

                else return null;
    }

    private void RequestNewShape()
    {
        currentShape.RequestNewShape();
    }


}
