namespace GameOfLife.Application.Services;

public interface IGameOfLifeService
{
    bool[][] GetNextState(bool[][] currentState);
    bool[][] GetStateAfter(bool[][] currentState, int steps);
    bool[][] GetFinalState(bool[][] currentState, int maxAttempts, out int attemptsTaken);
}