using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
   // public List<Shape> shapeList;

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
        // foreach(var shape in shapeList)
        // {
        //     var shapeIndex = UnityEngine.Random.Range(0,shapeData.Count);
        //     shape.CreateShape(shapeData[shapeIndex]);
        // }

        currentShape.CreateShape(shapeData[0]);
    }


    public Shape GetCurrentSelectedShape()
    {
         if (!currentShape.IsOnStartPosition() && currentShape.IsAnyOfShapeSquareActive())
                return currentShape;

                else return null;
    }

    private void RequestNewShape()
    {
        currentShape.RequestNewShape(shapeData[0]);
    }


}
