namespace Cursed.Character
{
    /// <summary>
    /// Interface Input Controller
    /// </summary>
    public interface IInputController
    {
        float x { get; }
        float y { get; }
        BoolBuffer Jump { get; }
        bool HoldJump { get; }
        BoolBuffer Dash { get; }
        bool Grab { get; }
        bool Attack { get; }
    }
}