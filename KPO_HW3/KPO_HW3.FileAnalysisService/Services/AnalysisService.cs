using DotNext;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using KPO_HW3.FileAnalysisService.Infrastructure.Extensions;

namespace KPO_HW3.FileAnalysisService.Services;

public class AnalysisService(
    IFileStorageApi fileStorageApi,
    AnalysisDbContext dbContext) : IAnalysisService
{
    public Task<Result<PlagiarismReportDto>> AnalyzeAsync(Guid workId, CancellationToken ct = default) =>
        fileStorageApi.GetWorkAsync(workId, ct)
            .ContinueAsync<WorkSnapshot, PlagiarismReportDto>(async snapshot =>
            {
                using var httpResponse = await fileStorageApi.GetWorkContentAsync(workId, ct);
                var stream = await httpResponse.Content.ReadAsStreamAsync(ct);
                string contentHash = await ComputeContentHash(stream, ct);

                var originalReports  = await dbContext.PlagiarismReports
                    .AsNoTracking()
                    .Include(r => r.Matches)
                    .Where(r =>
                        r.ContentHash == contentHash &&
                        r.SimilarityScore < 1 &&
                        r.StudentId != snapshot.StudentId)
                    .ToListAsync(cancellationToken: ct);

                var report = new PlagiarismReport
                {
                    WorkId = snapshot.Id,
                    StudentId = snapshot.StudentId,
                    ContentHash = contentHash,
                    SimilarityScore = originalReports.Count > 0 ? 1.0 : 0.0,
                    Matches = originalReports.Select(r => new PlagiarismReportMatch
                    {
                        SimilarityScore = 1,
                        MatchedWorkId = r.WorkId,
                    }).ToList()
                };

                dbContext.PlagiarismReports.Add(report);
                await dbContext.SaveChangesAsync(ct);

                return report.ToDto();
            });

    private static async Task<string> ComputeContentHash(Stream stream, CancellationToken ct)
    {
        using var sha = SHA256.Create();
        var hashBytes = await sha.ComputeHashAsync(stream, ct);
        string contentHash = Convert.ToHexString(hashBytes);
        return contentHash;
    }
}