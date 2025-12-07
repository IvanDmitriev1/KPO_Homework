using DotNext;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace KPO_HW3.FileStorageService.Services;

public class MinioFileStorage(IMinioClient client, string bucketName) : IFileStorage
{
    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        var exists = await client
            .BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), ct);

        if (!exists)
        {
            await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), ct);
        }

        var fileId = GenerateObjectKey(fileName);

        if (content.CanSeek)
            content.Position = 0;

        var size = content.CanSeek ? content.Length : -1;

        var putArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await client.PutObjectAsync(putArgs, ct);

        return fileId;
    }

    public async Task<Result<StorageFileInfo>> GetFileInfoAsync(string fileId, CancellationToken ct = default)
    {
        var statArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId);

        try
        {
            var stat = await client.StatObjectAsync(statArgs, ct);

            return new StorageFileInfo
            {
                FileId = fileId,
                FileName = stat.ObjectName,
                ContentType = stat.ContentType,
                Length = stat.Size
            };
        }
        catch (ObjectNotFoundException)
        {
            return Result.FromException<StorageFileInfo>(new FileNotFoundException());
        }
    }

    public async Task GetAsync(string fileId, Stream destination, CancellationToken ct = default)
    {
        var getArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId)
            .WithCallbackStream(async (s, cancellationToken) =>
            {
                await s.CopyToAsync(destination, 81920, cancellationToken);
            });

        await client.GetObjectAsync(getArgs, ct);
    }

    public async Task DeleteAsync(string fileId, CancellationToken ct = default)
    {
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId);

        await client.RemoveObjectAsync(removeArgs, ct);
    }

    private static string GenerateObjectKey(string fileName)
    {
        var datePrefix = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var guid = Guid.NewGuid().ToString("N");
        return $"{datePrefix}/{guid}-{fileName}";
    }
}