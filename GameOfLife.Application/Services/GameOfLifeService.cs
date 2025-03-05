namespace GameOfLife.Application.Services;

public class GameOfLifeService : IGameOfLifeService
{
    public bool[][] GetNextState(bool[][] currentState)
    {
        throw new NotImplementedException();
    }

    public bool[][] GetStateAfter(bool[][] currentState, int steps)
    {
        throw new NotImplementedException();
    }

    public bool[][] GetFinalState(bool[][] currentState, int maxAttempts, out int attemptsTaken)
    {
        throw new NotImplementedException();
    }
}