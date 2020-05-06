using System.Collections;
using UnityEngine;
using Cursed.Creature;

public class DoorSwitch : MonoBehaviour
{
    [SerializeField] private InteractiveDoor[] _interactiveDoors;

    public void ToggleDoors()
    {
        Debug.Log("Toggle");
        for(int i = 0; i < _interactiveDoors.Length; i++)
        {
            _interactiveDoors[i].ToggleDoor();
        }
    }
}
