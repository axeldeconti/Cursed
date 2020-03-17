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
        bool HoldRightTrigger { get; }
        bool Attack_1 { get; }
        bool Attack_2 { get; }
    }
}