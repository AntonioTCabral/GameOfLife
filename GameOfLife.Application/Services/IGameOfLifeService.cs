using GameOfLife.Application.DTOs;

namespace GameOfLife.Application.Services;

public interface IGameOfLifeService
{
    Task<BoardDTO> CreateBoardAsync(BoardUploadRequest request);
    Task<bool[][]> GetNextState(Guid boardId);
    Task<bool[][]> GetStateAfter(Guid boardId, int steps);
    Task<FinalStateDTO> GetFinalState(Guid boardId, int maxAttempts);
}