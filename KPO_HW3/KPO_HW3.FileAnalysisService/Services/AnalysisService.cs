using DotNext;
using KPO_HW3.FileAnalysisService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using KPO_HW3.FileAnalysisService.Infrastructure.Extensions;

namespace KPO_HW3.FileAnalysisService.Services;

public class AnalysisService(
    IFileStorageApi fileStorageApi,
    AnalysisDbContext dbContext) : IAnalysisService
{
    public Task<Result<PlagiarismReport>> AnalyzeAsync(Guid workId, CancellationToken ct = default) =>
        fileStorageApi.GetWorkAsync(workId, ct)
            .ContinueAsync<WorkSnapshot, PlagiarismReport>(async snapshot =>
            {
                await using var stream = await fileStorageApi.GetWorkContentAsync(workId, ct);
                string contentHash = await ComputeContentHash(stream, ct);

                bool plagiarized = await dbContext.PlagiarismReports.AnyAsync(r =>
                        r.AssignmentId == snapshot.AssignmentId &&
                        r.ContentHash == contentHash &&
                        r.StudentId != snapshot.StudentId,
                    cancellationToken: ct);

                var report = new PlagiarismReport
                {
                    WorkId = snapshot.WorkId,
                    StudentId = snapshot.StudentId,
                    AssignmentId = snapshot.AssignmentId,
                    ContentHash = contentHash,
                    IsPlagiarized = plagiarized,
                    SimilarityScore = plagiarized ? 1.0 : 0.0,
                    Details = plagiarized
                        ? "Detected identical content for the same assignment from another student."
                        : "No matching content found for this assignment.",
                };

                dbContext.PlagiarismReports.Add(report);
                await dbContext.SaveChangesAsync(ct);

                return report;
            });

    private static async Task<string> ComputeContentHash(Stream stream, CancellationToken ct)
    {
        using var sha = SHA256.Create();
        var hashBytes = await sha.ComputeHashAsync(stream, ct);
        string contentHash = Convert.ToHexString(hashBytes);
        return contentHash;
    }
}