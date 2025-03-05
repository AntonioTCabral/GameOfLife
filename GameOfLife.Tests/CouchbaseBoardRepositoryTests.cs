using Couchbase;
using GameOfLife.Application.DTOs;
using GameOfLife.Domain.Entities;
using GameOfLife.Infrastructure.Repositories;

namespace GameOfLife.Tests;

public class CouchbaseBoardRepositoryTests : IAsyncLifetime
{
    private ICluster _cluster;
    private BoardRepository _repository;
    private readonly CouchbaseSettings _settings;

    
    public CouchbaseBoardRepositoryTests()
    {
        // Set up the test Couchbase configuration.
        _settings = new CouchbaseSettings
        {
            ConnectionString = "couchbase://localhost", // or "127.0.0.1" if appropriate for your environment
            UserName = "admin",
            Password = "admin123456",
            BucketName = "gameOfLifeDB" // Use a bucket dedicated for tests
        };
    }

    
    public async Task InitializeAsync()
    {
        // Connect to the Couchbase cluster using test settings.
        _cluster = await Cluster.ConnectAsync(_settings.ConnectionString, _settings.UserName, _settings.Password);
        // Initialize the repository with the settings injected via IOptions.
        _repository = new BoardRepository(_cluster, Microsoft.Extensions.Options.Options.Create(_settings));
    }

   
    public async Task DisposeAsync()
    {
        if (_cluster != null)
        {
            await _cluster.DisposeAsync();
        }
    }

    /// <summary>
    /// Tests that creating a board and then retrieving it returns the same board.
    /// This test creates a board with a simple initial state, saves it to Couchbase,
    /// and then retrieves the board to verify that the ID and state remain unchanged.
    /// </summary>
    [Fact]
    public async Task CreateAndRetrieveBoard_ShouldReturnSameBoard()
    {
        // Arrange: Create a board with a simple initial state.
        bool[][] initialState =
        {
            new[] { false, true },
            new[] { true, false }
        };
        var board = new Board
        {
            Id = Guid.NewGuid(),
            State = initialState
        };

       
        await _repository.CreateBoardAsync(board);

        // Act: Retrieve the board that was just created.
        var retrievedBoard = await _repository.GetBoardAsync(board.Id);

        // Assert: Verify that the retrieved board is not null, its ID matches, and its state is identical.
        Assert.NotNull(retrievedBoard);
        Assert.Equal(board.Id, retrievedBoard.Id);
        Assert.True(Helper.CompareBoards(board.State, retrievedBoard.State));
    }

    /// <summary>
    /// Tests that updating a board persists the changes.
    /// This test creates a board, saves it to Couchbase, updates its state,
    /// and then retrieves the board to verify that the update was successfully persisted.
    /// </summary>
    [Fact]
    public async Task UpdateBoard_ShouldPersistChanges()
    {
        // Arrange: Create a board with an initial state and save it.
        bool[][] initialState =
        {
            new[] { false, true },
            new[] { true, false }
        };
        var board = new Board
        {
            Id = Guid.NewGuid(),
            State = initialState
        };
        await _repository.CreateBoardAsync(board);

        // Act: Update the board's state.
        bool[][] newState =
        {
            new[] { true, false },
            new[] { false, true }
        };
        board.UpdateState(newState);
        await _repository.UpdateBoardAsync(board);

        // Retrieve the board after the update.
        var updatedBoard = await _repository.GetBoardAsync(board.Id);

        // Assert: Verify that the updated board is not null, its ID is unchanged, and the new state matches.
        Assert.NotNull(updatedBoard);
        Assert.Equal(board.Id, updatedBoard.Id);
        Assert.True(Helper.CompareBoards(newState, updatedBoard.State));
    }
}