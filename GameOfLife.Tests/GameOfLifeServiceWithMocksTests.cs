using GameOfLife.Application.Repositories;
using GameOfLife.Application.Services;
using GameOfLife.Domain.Entities;
using Moq;

namespace GameOfLife.Tests;

public class GameOfLifeServiceWithMocksTests
{
    /// <summary>
    /// Tests that the next state of a Blinker oscillator is calculated correctly.
    /// The Blinker should change from a horizontal line to a vertical line after one iteration.
    /// </summary>
    [Fact]
    public async Task GetNextState_BlinkerOscillator_ReturnsCorrectNextState()
    {
        // Arrange: Generate a unique board ID and define the initial Blinker pattern (horizontal).
        var boardId = Guid.NewGuid();
        bool[][] blinkerInitial =
        {
            new[] { false, false, false },
            new[] { true, true, true },
            new[] { false, false, false }
        };

        // Define the expected state after one iteration (vertical Blinker).
        bool[][] expectedNextState =
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, false }
        };

       
        var board = new Board
        {
            Id = Guid.NewGuid(),
            State = blinkerInitial
        };

       
        var mockRepo = new Mock<IBoardRepository>();
        mockRepo.Setup(r => r.GetBoardAsync(boardId)).ReturnsAsync(board);
        mockRepo.Setup(r => r.UpdateBoardAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var service = new GameOfLifeService(mockRepo.Object);

        // Act: Calculate the next state of the board.
        var nextState = await service.GetNextState(boardId);

        // Assert: Verify that the calculated next state matches the expected vertical Blinker state.
        Assert.True(Helper.CompareBoards(expectedNextState, nextState));
        mockRepo.Verify(r => r.UpdateBoardAsync(It.Is<Board>(b => Helper.CompareBoards(expectedNextState, b.State))),
            Times.Once());
    }

    /// <summary>
    /// Tests that a Blinker oscillator returns to its original state after two iterations.
    /// The Blinker oscillates between two states, so after two iterations the state should be the same as the initial state.
    /// </summary>
    [Fact]
    public async Task GetStateAfter_BlinkerOscillator_ReturnsOriginalStateAfterTwoIterations()
    {
        // Arrange: Define the initial Blinker pattern.
        var boardId = Guid.NewGuid();
        bool[][] blinkerInitial =
        {
            new[] { false, false, false },
            new[] { true, true, true },
            new[] { false, false, false }
        };

        // Create a board with the initial Blinker state.
        var board = new Board
        {
            Id = Guid.NewGuid(),
            State = blinkerInitial
        };

        
        var mockRepo = new Mock<IBoardRepository>();
        mockRepo.Setup(r => r.GetBoardAsync(boardId)).ReturnsAsync(board);
        mockRepo.Setup(r => r.UpdateBoardAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var service = new GameOfLifeService(mockRepo.Object);

        // Act: After two iterations, the Blinker should return to its original state.
        var stateAfterTwo = await service.GetStateAfter(boardId, 2);

        // Assert: Verify that the state after two iterations is identical to the initial state.
        Assert.True(Helper.CompareBoards(blinkerInitial, stateAfterTwo));
        mockRepo.Verify(r => r.UpdateBoardAsync(It.Is<Board>(b => Helper.CompareBoards(blinkerInitial, b.State))),
            Times.Once());
    }

    /// <summary>
    /// Tests that the Glider pattern evolves correctly after four iterations.
    /// In a finite board, the Glider pattern should reach an expected configuration after four iterations.
    /// </summary>
    [Fact]
    public async Task GetStateAfter_GliderPattern_ReturnsExpectedStateAfterFourIterations()
    {
        // Arrange: Define the initial Glider pattern.
        var boardId = Guid.NewGuid();
        bool[][] gliderInitial =
        {
            new[] { false, true, false, false, false },
            new[] { false, false, true, false, false },
            new[] { true, true, true, false, false },
            new[] { false, false, false, false, false },
            new[] { false, false, false, false, false }
        };

        // Define the expected state after four iterations for the Glider.
        bool[][] expectedGliderStateAfter4 =
        {
            new[] { false, false, false, false, false },
            new[] { false, false, true, false, false },
            new[] { false, false, false, true, false },
            new[] { false, true, true, true, false },
            new[] { false, false, false, false, false }
        };

        // Create a board with the initial Glider pattern.
        var board = new Board
        {
            Id = boardId,
            State = gliderInitial
        };

        // Set up the mocked repository.
        var mockRepo = new Mock<IBoardRepository>();
        mockRepo.Setup(r => r.GetBoardAsync(boardId)).ReturnsAsync(board);
        mockRepo.Setup(r => r.UpdateBoardAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var service = new GameOfLifeService(mockRepo.Object);

        // Act: Evolve the board for four iterations.
        var stateAfterFour = await service.GetStateAfter(boardId, 4);

        // Assert: Verify that the resulting state matches the expected Glider state.
        Assert.True(Helper.CompareBoards(expectedGliderStateAfter4, stateAfterFour));
        mockRepo.Verify(
            r => r.UpdateBoardAsync(It.Is<Board>(b => Helper.CompareBoards(expectedGliderStateAfter4, b.State))),
            Times.Once());
    }

    /// <summary>
    /// Tests that a stable Block pattern remains unchanged after applying the next state operation.
    /// The Block is a stable configuration, so its state should remain identical.
    /// </summary>
    [Fact]
    public async Task GetNextState_BlockPattern_ReturnsSameState()
    {
        // Arrange: Define the initial Block pattern which is stable.
        var boardId = Guid.NewGuid();
        bool[][] blockState =
        {
            new[] { false, false, false, false },
            new[] { false, true, true, false },
            new[] { false, true, true, false },
            new[] { false, false, false, false }
        };

        // Create a board with the Block pattern.
        var board = new Board
        {
            Id = boardId,
            State = blockState
        };

        // Set up the mocked repository.
        var mockRepo = new Mock<IBoardRepository>();
        mockRepo.Setup(r => r.GetBoardAsync(boardId)).ReturnsAsync(board);
        mockRepo.Setup(r => r.UpdateBoardAsync(It.IsAny<Board>())).Returns(Task.CompletedTask);

        var service = new GameOfLifeService(mockRepo.Object);

        // Act: Retrieve the next state, which for a stable Block should be the same as the initial state.
        var nextState = await service.GetNextState(boardId);

        // Assert: Verify that the next state matches the original Block state.
        Assert.True(Helper.CompareBoards(blockState, nextState));
        mockRepo.Verify(r => r.UpdateBoardAsync(It.Is<Board>(b => Helper.CompareBoards(blockState, b.State))),
            Times.Once());
    }
}