using GameOfLife.Application.DTOs;
using GameOfLife.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife_NewVersion.Controllers;


[ApiController]
[Route("board")]
public class BoardController : ControllerBase
{
    private readonly IGameOfLifeService _gameOfLifeService;

    
    public BoardController(IGameOfLifeService gameOfLifeService)
    {
        _gameOfLifeService = gameOfLifeService;
    }

    /// <summary>
    /// Uploads a new board state and creates a board.
    /// </summary>
    /// <param name="request">The board upload request containing the initial state.</param>
    /// <returns>
    /// An action result with the created board object (including its unique ID) if the state is valid;
    /// otherwise, a Bad Request response is returned.
    /// </returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(BoardDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadBoard([FromBody] BoardUploadRequest request)
    {
        if (request.State == null || request.State.Length == 0)
            return BadRequest("Invalid board state");

        var board = await _gameOfLifeService.CreateBoardAsync(request);

        return Ok(board);
    }

    /// <summary>
    /// Returns the next state of the board based on its current state.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <returns>An action result with the next state of the board.</returns>
    [HttpGet("{id:guid}/next")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNextState(Guid id)
    {
        return Ok(await _gameOfLifeService.GetNextState(id));
    }

    /// <summary>
    /// Returns the state of the board after the specified number of steps.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <param name="steps">The number of iterations to advance the board.</param>
    /// <returns>An action result with the board state after the given number of steps.</returns>
    [HttpGet("{id:guid}/states/{steps:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStateAfter(Guid id, int steps)
    {
        return Ok(await _gameOfLifeService.GetStateAfter(id, steps));
    }

    /// <summary>
    /// Returns the final state of the board if it reaches a stable state or loop within the specified maximum attempts.
    /// </summary>
    /// <param name="id">The unique identifier of the board.</param>
    /// <param name="maxAttempts">
    /// The maximum number of iterations to attempt reaching a final state.
    /// Defaults to 100.
    /// </param>
    /// <returns>
    /// An action result with the final state of the board and the number of attempts taken,
    /// or an error if the final state is not reached within the maximum attempts.
    /// </returns>
    [HttpGet("{id:guid}/final")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinalState(Guid id, [FromQuery] int maxAttempts = 100)
    {
        return Ok(await _gameOfLifeService.GetFinalState(id, maxAttempts));
    }
}