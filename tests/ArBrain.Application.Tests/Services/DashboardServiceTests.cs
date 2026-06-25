using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Application.Services;
using ArBrain.Domain.Enums;
using Moq;

namespace ArBrain.Application.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_AggregatesComplianceCounts()
    {
        var repository = new Mock<IFermentationRecordRepository>();
        repository
            .Setup(repo => repo.GetComplianceCountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<FermentationComplianceStatus, int>
            {
                [FermentationComplianceStatus.WithinStandard] = 5,
                [FermentationComplianceStatus.RequiresAttention] = 2,
                [FermentationComplianceStatus.OutOfStandard] = 1,
            });

        var service = new DashboardService(repository.Object);

        var summary = await service.GetSummaryAsync();

        Assert.Equal(8, summary.TotalRecords);
        Assert.Equal(5, summary.WithinStandardCount);
        Assert.Equal(2, summary.RequiresAttentionCount);
        Assert.Equal(1, summary.OutOfStandardCount);
    }

    [Fact]
    public async Task GetSummaryAsync_WhenCountsAreMissing_ReturnsZeroForMissingStatuses()
    {
        var repository = new Mock<IFermentationRecordRepository>();
        repository
            .Setup(repo => repo.GetComplianceCountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<FermentationComplianceStatus, int>
            {
                [FermentationComplianceStatus.WithinStandard] = 3,
            });

        var service = new DashboardService(repository.Object);

        var summary = await service.GetSummaryAsync();

        Assert.Equal(3, summary.TotalRecords);
        Assert.Equal(3, summary.WithinStandardCount);
        Assert.Equal(0, summary.RequiresAttentionCount);
        Assert.Equal(0, summary.OutOfStandardCount);
    }
}
