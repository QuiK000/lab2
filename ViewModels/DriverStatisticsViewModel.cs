using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class DriverStatisticsViewModel
    {
        public Driver Driver { get; set; } = null!;
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<DailyOrderStats> Last7Days { get; set; } = new();
        public List<Review> RecentReviews { get; set; } = new();
    }

    public class DailyOrderStats
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}