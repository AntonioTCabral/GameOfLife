namespace GameOfLife.Domain.Entities;

public class Board
{
    public Guid Id { get; init; }
    public bool[][] CurrentState { get; set; }
}