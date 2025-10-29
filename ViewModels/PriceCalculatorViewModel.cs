using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class PriceCalculatorViewModel
    {
        public List<Service> Services { get; set; } = new();
        public decimal Distance { get; set; }
        public int? SelectedServiceId { get; set; }
        public Dictionary<int, decimal> CalculatedPrices { get; set; } = new();
    }
}
