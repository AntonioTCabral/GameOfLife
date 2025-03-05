using GameOfLife.Domain.Entities;

namespace GameOfLife.Application.Repositories;

public interface IBoardRepository
{
    Task<Board> CreateBoardAsync(Board board);
    Task<Board?> GetBoardAsync(Guid id);
    Task UpdateBoardAsync(Board board);
}