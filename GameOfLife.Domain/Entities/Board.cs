namespace GameOfLife.Domain.Entities;

public class Board
{
    public Guid Id { get; init; }
    public bool[][] State { get; set; }
    
    public void UpdateState(bool[][] newState)
    {
        State = newState;
    }
}