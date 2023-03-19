namespace BooksRentalSystem.Snapshottings.Minio.Options;

public record MinioOptions
{
    public string Endpoint { get; init; }
    public string AccessKey { get; init; }
    public string SecretKey { get; init; }
}