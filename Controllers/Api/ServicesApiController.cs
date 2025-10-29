using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;

namespace WebApplication2.Controllers.Api
{
    [Route("api/services")]
    [ApiController]
    public class ServicesApiController : ControllerBase
    {
        private readonly TaxiContext _context;

        public ServicesApiController(TaxiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _context.Services
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.BasePrice,
                    s.PricePerKm,
                    s.CarType
                })
                .ToListAsync();

            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _context.Services
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.BasePrice,
                    s.PricePerKm,
                    s.CarType
                })
                .FirstOrDefaultAsync();

            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculatePriceRequest request)
        {
            var service = await _context.Services.FindAsync(request.ServiceId);
            if (service == null)
                return NotFound();

            var totalPrice = service.BasePrice + (service.PricePerKm * request.Distance);

            return Ok(new
            {
                serviceId = service.Id,
                serviceName = service.Name,
                distance = request.Distance,
                basePrice = service.BasePrice,
                pricePerKm = service.PricePerKm,
                totalPrice = totalPrice
            });
        }
    }

    public class CalculatePriceRequest
    {
        public int ServiceId { get; set; }
        public decimal Distance { get; set; }
    }
}