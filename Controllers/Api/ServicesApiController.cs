using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.db;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Controllers.Api
{
    [Route("api/[controller]")]
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { error = "Invalid service ID" });
            }

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
                return NotFound(new { error = $"Service with ID {id} not found" });

            return Ok(service);
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculatePriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.ServiceId <= 0)
            {
                return BadRequest(new { error = "Invalid service ID" });
            }

            if (request.Distance <= 0)
            {
                return BadRequest(new { error = "Distance must be greater than zero" });
            }

            var service = await _context.Services.FindAsync(request.ServiceId);
            if (service == null)
                return NotFound(new { error = $"Service with ID {request.ServiceId} not found" });

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
        [Required(ErrorMessage = "Service ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Service ID must be greater than 0")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Distance is required")]
        [Range(0.1, 10000, ErrorMessage = "Distance must be between 0.1 and 10000 km")]
        public decimal Distance { get; set; }
    }
}