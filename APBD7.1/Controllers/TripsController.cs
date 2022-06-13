using System.Threading.Tasks;
using APBD7._1.Services;
using Microsoft.AspNetCore.Mvc;
using APBD7._1.Models.DTO;

namespace APBD7._1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public TripsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _dbService.GetTrips();
            return Ok(trips);
        }

        [HttpDelete]
        [Route("{idClient}")]
        public async Task<IActionResult> RemoveClient(int idClient) 
        {
            var client = await _dbService.DeleteClient(idClient);
            if (client)
            {
                return Ok($"Client has been removed");
            }
            else
            {
                return BadRequest("Cannot delete client because some trips are connected");
            }
            
        }

        [HttpPost]
        [Route("{idTrip}/clients")]
        public async Task<IActionResult> AddClientToTrip(ClientToTrip clientToTrip, int idTrip) 
        {
            await _dbService.CheckPesel(clientToTrip);

            if (_dbService.CheckIfTripExists(clientToTrip).Result == true) {
                return BadRequest("Trip does not exist in DB");
            }
            if (_dbService.CheckIfClientIsRegistered(clientToTrip).Result == true) 
            {
                return BadRequest($"Client is already reigistered");
            }
            await _dbService.AddClientToTrip(clientToTrip);
            return Ok($"Client {clientToTrip.FirstName} {clientToTrip.LastName} has been addes to trip {idTrip}");
        }

    }
}
