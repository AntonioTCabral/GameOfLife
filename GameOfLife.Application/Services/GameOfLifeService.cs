using GameOfLife.Application.DTOs;
using GameOfLife.Application.Repositories;
using GameOfLife.Domain.Entities;

namespace GameOfLife.Application.Services;

public class GameOfLifeService : IGameOfLifeService
{
    private readonly IBoardRepository _boardRepository;

    public GameOfLifeService(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<BoardDTO> CreateBoardAsync(BoardUploadRequest request)
    {
        var board = new Board
        {
            Id = Guid.NewGuid(),
            State = request.State,
        };
        
        await _boardRepository.CreateBoardAsync(board);
        
        return new BoardDTO(board.Id, board.State);
    }

    public async Task<bool[][]> GetNextState(Guid boardId)
    {
        var board = await _boardRepository.GetBoardAsync(boardId);
        if (board == null)
            throw new KeyNotFoundException($"Board with id {boardId} not found.");

        var nextState = CalculateNextState(board.State);
        board.UpdateState(nextState);
        await _boardRepository.UpdateBoardAsync(board);
        return nextState;
    }

    public async Task<bool[][]> GetStateAfter(Guid boardId, int steps)
    {
        var board = await _boardRepository.GetBoardAsync(boardId);
        if (board == null)
            throw new KeyNotFoundException($"Board with id {boardId} not found.");

        bool[][] currentState = board.State;
        for (int i = 0; i < steps; i++)
        {
            currentState = CalculateNextState(currentState);
        }

        board.UpdateState(currentState);
        await _boardRepository.UpdateBoardAsync(board);
        return currentState;
    }

    public async Task<FinalStateDTO> GetFinalState(Guid boardId, int maxAttempts)
    {
        var board = await _boardRepository.GetBoardAsync(boardId);
        if (board == null)
            throw new KeyNotFoundException($"Board with id {boardId} not found.");

        bool[][] currentState = board.State;
        var seenStates = new HashSet<string>();
        string stateKey = SerializeState(currentState);
        seenStates.Add(stateKey);
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            currentState = CalculateNextState(currentState);
            attempts++;

            stateKey = SerializeState(currentState);
            if (seenStates.Contains(stateKey))
            {
                board.UpdateState(currentState);
                await _boardRepository.UpdateBoardAsync(board);
                return new FinalStateDTO(currentState, attempts);
            }

            seenStates.Add(stateKey);
        }

        throw new InvalidOperationException(
            $"Não foi possível atingir um estado final após {maxAttempts} tentativas.");
    }

    // Método auxiliar para calcular o próximo estado do tabuleiro
    private bool[][] CalculateNextState(bool[][] currentState)
    {
        int rows = currentState.Length;
        int cols = currentState[0].Length;
        bool[][] nextState = new bool[rows][];

        for (int i = 0; i < rows; i++)
        {
            nextState[i] = new bool[cols];
            for (int j = 0; j < cols; j++)
            {
                int liveNeighbors = CountLiveNeighbors(currentState, i, j, rows, cols);

                // Regras do Jogo da Vida:
                if (currentState[i][j])
                    nextState[i][j] = (liveNeighbors == 2 || liveNeighbors == 3);
                else
                    nextState[i][j] = (liveNeighbors == 3);
            }
        }

        return nextState;
    }

    // Conta os vizinhos vivos de uma célula
    private int CountLiveNeighbors(bool[][] board, int row, int col, int rows, int cols)
    {
        int count = 0;
        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = col - 1; j <= col + 1; j++)
            {
                if (i == row && j == col)
                    continue;
                if (i >= 0 && i < rows && j >= 0 && j < cols && board[i][j])
                    count++;
            }
        }

        return count;
    }

    // Serializa o estado do tabuleiro para uma string, facilitando a comparação
    private string SerializeState(bool[][] state)
    {
        return string.Join(";", state.Select(row => string.Join(",", row.Select(c => c ? "1" : "0"))));
    }
}