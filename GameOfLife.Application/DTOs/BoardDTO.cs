namespace GameOfLife.Application.DTOs;

public record BoardDTO(Guid Id, bool[][] CurrentState);