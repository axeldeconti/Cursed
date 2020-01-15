using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public FloatReference test;
    private bool _cursor = true;

    private void Start()
    {
        ShowMouseCursor();
    }

    public void ShowMouseCursor()
    {
        if (_cursor)
        {
            Cursor.visible = false;
            _cursor = false;
        }
        else
        {
            Cursor.visible = true;
            _cursor = true;
        }
    }
}
