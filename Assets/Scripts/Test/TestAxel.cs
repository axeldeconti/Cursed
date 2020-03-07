using UnityEngine;

public class TestAxel : MonoBehaviour
{
    [SerializeField][Range(0, 1)] private float _amount = 1;
    [SerializeField] private IntEvent _testIntEvent = null;

    public void Test()
    {
        _testIntEvent.Raise((int)(_amount * 100));
    }
}
