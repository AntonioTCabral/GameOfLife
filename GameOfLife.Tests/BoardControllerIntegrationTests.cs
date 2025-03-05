using System.Net.Http.Json;
using GameOfLife.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GameOfLife.Tests;

public class BoardControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;


    public BoardControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Tests the creation of a board and retrieving its next state.
    /// This test uploads an initial board state using a "Blinker" pattern, retrieves the created board's ID,
    /// then calls the endpoint to obtain the next state and verifies that the state has been updated correctly.
    /// </summary>
    [Fact]
    public async Task CreateBoard_And_GetNextState_IntegrationTest()
    {
        // Arrange: Define an initial board state using a Blinker pattern.
        var boardUploadRequest = new BoardUploadRequest(
            new[]
            {
                new[] { false, false, false },
                new[] { true, true, true },
                new[] { false, false, false }
            });

        // Act: Send a POST request to create the board with the given initial state.
        var createResponse = await _client.PostAsJsonAsync("/board/upload", boardUploadRequest);
        createResponse.EnsureSuccessStatusCode();

        // Retrieve the board Id from the response.
        var createResponseObject = await createResponse.Content.ReadFromJsonAsync<BoardDTO>();
        Assert.NotNull(createResponseObject);
        var boardId = createResponseObject.Id;

        // Act: Call the endpoint to get the next state of the board.
        var nextStateResponse = await _client.GetAsync($"/board/{boardId}/next");
        nextStateResponse.EnsureSuccessStatusCode();

     
        var nextState = await nextStateResponse.Content.ReadFromJsonAsync<bool[][]>();

        // Assert: Verify that the returned state is not null and has the expected dimensions.
        Assert.NotNull(nextState);
        Assert.Equal(3, nextState.Length);
        Assert.Equal(3, nextState[0].Length);

        // Expected next state for a Blinker pattern after one iteration (vertical orientation).
        bool[][] expectedNextState = new[]
        {
            new[] { false, true, false },
            new[] { false, true, false },
            new[] { false, true, false }
        };

        // Assert that the returned state matches the expected state.
        Assert.True(Helper.CompareBoards(expectedNextState, nextState));
    }

    /// <summary>
    /// Tests that a stable "Block" pattern remains unchanged after several iterations.
    /// This test creates a board with the Block pattern, then retrieves its state after 5 iterations.
    /// Since the Block pattern is stable, the state should remain the same as the initial state.
    /// </summary>
    [Fact]
    public async Task GetStateAfter_BlockPattern_IntegrationTest()
    {
        // Arrange: Define the initial state for a stable Block pattern.
        var boardUploadRequest = new BoardUploadRequest
        (
            new[]
            {
                new[] { false, false, false, false },
                new[] { false, true,  true,  false },
                new[] { false, true,  true,  false },
                new[] { false, false, false, false }
            }
        );

        // Act: Send a POST request to create the board.
        var createResponse = await _client.PostAsJsonAsync("/board/upload", boardUploadRequest);
        createResponse.EnsureSuccessStatusCode();

      
        var createResponseObject = await createResponse.Content.ReadFromJsonAsync<BoardDTO>();
        Assert.NotNull(createResponseObject);
        var boardId = createResponseObject.Id;

        // Act: Call the endpoint to retrieve the board state after 5 iterations.
        var stateAfterResponse = await _client.GetAsync($"/board/{boardId}/states/5");
        stateAfterResponse.EnsureSuccessStatusCode();

        
        var stateAfter = await stateAfterResponse.Content.ReadFromJsonAsync<bool[][]>();

        // Assert: Verify that the state after 5 iterations is identical to the initial state.
        Assert.True(Helper.CompareBoards(boardUploadRequest.State, stateAfter));
    }
}
