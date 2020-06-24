using Boo.Lang;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cursed.Managers
{
    public class InputSystemManager : Singleton<InputSystemManager>
    {
        private List<StandaloneInputModule> _inputModules = new List<StandaloneInputModule>();

        private void RemoveAllObject()
        {
            foreach(StandaloneInputModule inputModule in _inputModules)
            {
                _inputModules.Remove(inputModule);
                Debug.Log("Remove " + inputModule + " to the Input Module list");
            }
        }

        public void GetInputModuleFromNewScene()
        {
            RemoveAllObject();
            foreach(StandaloneInputModule inputModule in FindObjectsOfType<StandaloneInputModule>())
            {
                _inputModules.Add(inputModule);
                Debug.Log("Add " + inputModule + " to the Input Module list");
            }
        }

    }
}
