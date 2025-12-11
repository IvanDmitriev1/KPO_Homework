namespace KPO_HW3.FileStorageService.Models;

public sealed class StorageFileInfo
{
    public required string ContentType { get; init; }
    public required Int64 Length { get; init; }
}