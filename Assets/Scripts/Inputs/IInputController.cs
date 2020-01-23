using UnityEngine;

namespace Cursed.Character
{
    /// <summary>
    /// Interface Input Controller
    /// </summary>
    public interface IInputController
    {
        float x { get; }
        float y { get; }
        float xRaw { get; }
        float yRaw { get; }
        bool Jump { get; }
        bool Dash { get; }
        bool Grab { get; }
        bool Attack { get; }
    }
}