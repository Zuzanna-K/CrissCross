using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] //Informuje Unity, że ta klasa ScriptableObject powinna być
// dostępna w menu kontekstowym w Unity Editor, co ułatwia tworzenie instancji tego obiektu.
[System.Serializable] //może zawierać dodatkowe parametry, takie jak nazwa pliku czy menu, w którym powinien się pojawić.

public class ShapeData : ScriptableObject
{
    [System.Serializable]
    public class Row
    {
        public bool[]column;
        private int thisSize = 0;

        public Row(){}
        public Row(int size)
        {
            CreateRow(size);
        }

        public void CreateRow(int size)
        {
            thisSize = size;
            column = new bool[thisSize];
            ClearRow();
        }

        public void ClearRow()
        {
            for (int i =0 ; i < thisSize ; i++ )
                {
                    column[i]= false;
                }
        }

    }

    public int columns = 0;
    public int rows = 0;
    public Row[]board;

    public void Clear()
    {
        for(var i =0 ; i < rows ; i++ )
        {
            board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        board = new Row[rows];

        for (var i = 0;i<rows;i++)
        {
            board[i] = new Row(columns);
        }
    }

}
