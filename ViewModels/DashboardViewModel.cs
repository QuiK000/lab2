using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalCars { get; set; }

        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public int MonthOrders { get; set; }
        public decimal MonthRevenue { get; set; }

        public int NewOrders { get; set; }
        public int InProgressOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }

        public List<DriverStats> TopDrivers { get; set; } = new();
        public List<ServiceStats> PopularServices { get; set; } = new();
        public List<DailyStats> DailyStats { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }

    public class DriverStats
    {
        public string Name { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ServiceStats
    {
        public string Name { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyStats
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}