using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.UI;

public class MapExitCollider : MonoBehaviour
{
    [SerializeField] private MapUIManager _mapUI;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _mapUI.DeactiveMap();
        }
    }
}
