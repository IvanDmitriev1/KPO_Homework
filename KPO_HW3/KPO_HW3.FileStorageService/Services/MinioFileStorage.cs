using DotNext;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace KPO_HW3.FileStorageService.Services;

public class MinioFileStorage(IMinioClient client, string bucketName) : IFileStorage
{
    public async Task SaveAsync(
        Stream content,
        Guid fileId,
        string contentType,
        CancellationToken ct = default)
    {
        var exists = await client
            .BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), ct);

        if (!exists)
        {
            await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), ct);
        }

        if (content.CanSeek)
            content.Position = 0;

        var size = content.CanSeek ? content.Length : -1;

        var putArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId.ToString())
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await client.PutObjectAsync(putArgs, ct);
    }

    public async Task<Result<StorageFileInfo>> GetFileInfoAsync(Guid fileId, CancellationToken ct = default)
    {

        var statArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId.ToString());

        try
        {
            var stat = await client.StatObjectAsync(statArgs, ct);

            return new StorageFileInfo
            {
                ContentType = stat.ContentType,
                Length = stat.Size
            };
        }
        catch (ObjectNotFoundException)
        {
            return Result.FromException<StorageFileInfo>(new FileNotFoundException());
        }
    }

    public async Task GetAsync(Guid fileId, Stream destination, CancellationToken ct = default)
    {
        var getArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId.ToString())
            .WithCallbackStream(async (s, cancellationToken) =>
            {
                await s.CopyToAsync(destination, 81920, cancellationToken);
            });

        await client.GetObjectAsync(getArgs, ct);
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken ct = default)
    {
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileId.ToString());

        await client.RemoveObjectAsync(removeArgs, ct);
    }
}