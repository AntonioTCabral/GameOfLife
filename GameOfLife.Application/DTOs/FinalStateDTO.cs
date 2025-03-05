namespace GameOfLife.Application.DTOs;

public record FinalStateDTO(bool[][] finalState, int attemptsTaken);