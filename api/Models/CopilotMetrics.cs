namespace AiUsageDashboard.Api.Models;

public class DailyMetrics
{
    public string Date { get; set; } = string.Empty;
    public int TotalSuggestionsCount { get; set; }
    public int TotalAcceptancesCount { get; set; }
    public int TotalLinesSuggested { get; set; }
    public int TotalLinesAccepted { get; set; }
    public int TotalActiveUsers { get; set; }
    public List<MetricsBreakdown> Breakdown { get; set; } = [];
}

public class MetricsBreakdown
{
    public string Language { get; set; } = string.Empty;
    public string Editor { get; set; } = string.Empty;
    public int SuggestionsCount { get; set; }
    public int AcceptancesCount { get; set; }
    public int LinesSuggested { get; set; }
    public int LinesAccepted { get; set; }
    public int ActiveUsers { get; set; }
}
