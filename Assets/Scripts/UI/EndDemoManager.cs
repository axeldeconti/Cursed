using UnityEngine;
using Cursed.Managers;

namespace Cursed.UI
{
    public class EndDemoManager : MonoBehaviour
    {
        public void BackToMenu()
        {
            GameManager.Instance.LoadLevel("Main", true);
        }
    }
}
