namespace The_World.Engine.StateMachine;

/// <summary>
/// A push-down state machine. The state on top of the stack decides which
/// commands exist and what the prompt looks like. Pushing suspends the
/// current state; popping resumes it (OnResume) right where it left off:
///
///   Exploring -> talk elder     => push Dialogue
///   Dialogue  -> "show wares"   => push Shopping
///   Shopping  -> leave          => pop, Dialogue resumes
///   Dialogue  -> farewell       => pop, Exploring resumes
/// </summary>
public class GameStateMachine
{
    private readonly Stack<IGameState> _stack = new();

    /// <summary>The active state. Throws if the machine is empty (the engine always seeds one).</summary>
    public IGameState Current => _stack.Peek();

    public int Depth => _stack.Count;

    public bool IsEmpty => _stack.Count == 0;

    public void Push(GameContext ctx, IGameState state)
    {
        _stack.Push(state);
        state.OnEnter(ctx);
    }

    /// <summary>
    /// Pop the top state. The state beneath (if any) gets OnResume.
    /// </summary>
    public IGameState Pop(GameContext ctx)
    {
        var popped = _stack.Pop();
        popped.OnExit(ctx);
        if (_stack.Count > 0)
            _stack.Peek().OnResume(ctx);
        return popped;
    }

    /// <summary>
    /// Swap the top state for another without resuming what's beneath.
    /// </summary>
    public void Replace(GameContext ctx, IGameState state)
    {
        var popped = _stack.Pop();
        popped.OnExit(ctx);
        _stack.Push(state);
        state.OnEnter(ctx);
    }

    /// <summary>
    /// Clear the whole stack (no OnResume calls - the world is being torn down).
    /// </summary>
    public void Reset(GameContext ctx)
    {
        while (_stack.Count > 0)
            _stack.Pop().OnExit(ctx);
    }
}
