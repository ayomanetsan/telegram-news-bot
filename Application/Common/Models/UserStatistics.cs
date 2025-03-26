namespace Application.Common.Models;

public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int TotalInteractions { get; set; }
    public List<ThemePopularity> ThemePopularity { get; set; }
}   