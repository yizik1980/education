using boarding_school_api.Controllers;
using boarding_school_api.Infrastructure;
using boarding_school_api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace boarding_school_api.Tests;

public class BoardingSchoolsControllerTests
{
    private readonly Mock<IBoardingSchoolQuery> _queryMock = new();
    private readonly Mock<IEducationPlaceSummaryRepo> _summaryMock = new();
    private readonly BoardingSchoolsController _controller;

    public BoardingSchoolsControllerTests()
    {
        _controller = new BoardingSchoolsController(_queryMock.Object, _summaryMock.Object);
    }

    // ── GET /api/boardingschools ───────────────────────────────────────────

    [Fact]
    public async Task Get_ReturnsOkWithSchools()
    {
        var schools = new List<ActiveStudentsByPlace>
        {
            new() { EducationPlaceId = 1, PlaceName = "פנימייה א", ActiveStudentCount = 20, city = "תל אביב", AvrageAges = 15 },
            new() { EducationPlaceId = 2, PlaceName = "פנימייה ב", ActiveStudentCount = 15, city = "חיפה",    AvrageAges = 16 }
        };
        _queryMock.Setup(q => q.GetAllBoardingSchoolsSPAsync()).ReturnsAsync(schools);

        var result = await _controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
        var data = Assert.IsAssignableFrom<IEnumerable<ActiveStudentsByPlace>>(ok.Value);
        Assert.Equal(2, data.Count());
    }

    [Fact]
    public async Task Get_EmptyList_ReturnsOkWithEmptyCollection()
    {
        _queryMock.Setup(q => q.GetAllBoardingSchoolsSPAsync())
                  .ReturnsAsync(Enumerable.Empty<ActiveStudentsByPlace>());

        var result = await _controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);
    }

    // ── POST /api/boardingschools/summary ─────────────────────────────────

    [Fact]
    public async Task Post_Summary_ReturnsOkWithResult()
    {
        var summary = new List<EducationPlaceSummary>
        {
            new() { EducationPlaceId = 1, PlaceName = "פנימייה א", ActiveStudentCount = 20, city = "תל אביב" }
        };
        _summaryMock.Setup(s => s.GetSummaryAsync("תל אביב", 10)).ReturnsAsync(summary);

        var result = await _controller.Post(new BoardeSchoolRequest("תל אביב", 10));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task Post_Summary_NoFilter_ReturnsOkWithAllResults()
    {
        var summary = new List<EducationPlaceSummary>
        {
            new() { EducationPlaceId = 1, PlaceName = "פנימייה א", ActiveStudentCount = 20 },
            new() { EducationPlaceId = 2, PlaceName = "פנימייה ב", ActiveStudentCount = 10 }
        };
        _summaryMock.Setup(s => s.GetSummaryAsync(null, 0)).ReturnsAsync(summary);

        var result = await _controller.Post(new BoardeSchoolRequest(null!, 0));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    // ── POST /api/boardingschools/critical-incident ───────────────────────

    [Fact]
    public void TriggerCriticalIncident_AlwaysThrowsException()
    {
        var ex = Assert.Throws<Exception>(() => _controller.TriggerCriticalIncident("תקלה קריטית"));

        Assert.Contains("תקלה קריטית", ex.Message);
    }
}
