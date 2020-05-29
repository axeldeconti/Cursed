using System.Collections;
using UnityEngine;
using Cursed.Creature;

public class DoorSwitch : MonoBehaviour
{
    [SerializeField] private InteractiveDoor[] _interactiveDoors;
    public event System.Action _toggleDoor;

    public void ToggleDoors()
    {
        for(int i = 0; i < _interactiveDoors.Length; i++)
        {
            _interactiveDoors[i].ToggleDoor();
        }
        _toggleDoor?.Invoke();
    }
}
