using Couchbase;
using Couchbase.KeyValue;
using GameOfLife.Application.DTOs;
using GameOfLife.Application.Repositories;
using GameOfLife.Domain.Entities;
using Microsoft.Extensions.Options;

namespace GameOfLife.Infrastructure.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly ICluster _cluster;
    private readonly IBucket _bucket;
    private readonly ICouchbaseCollection _collection;

    public BoardRepository(ICluster cluster, IOptions<CouchbaseSettings> couchbaseSettings)
    {
        var settings = couchbaseSettings.Value;
        _cluster = cluster;
        _bucket = _cluster.BucketAsync(settings.BucketName).Result;
        _collection = _bucket.DefaultCollection();
    }


    public async Task<Board> CreateBoardAsync(Board board)
    {
        await _collection.InsertAsync(board.Id.ToString(), board);
        return board;
    }

    public async Task<Board?> GetBoardAsync(Guid id)
    {
        try
        {
            var result = await _collection.GetAsync(id.ToString());
            return result.ContentAs<Board>();
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task UpdateBoardAsync(Board board)
    {
        await _collection.UpsertAsync(board.Id.ToString(), board);
    }
}