namespace GameOfLife.Application.DTOs;

public class CouchbaseSettings
{
    public string ConnectionString { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string BucketName { get; set; }
}