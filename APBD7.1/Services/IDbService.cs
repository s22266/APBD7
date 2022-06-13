using System.Collections.Generic;
using System.Threading.Tasks;
using APBD7._1.Models;
using APBD7._1.Models.DTO;

namespace APBD7._1.Services
{
    public interface IDbService
    {
        Task<IEnumerable<object>> GetTrips();
        Task<bool> DeleteClient(int idClient);
        Task AddClientToTrip(ClientToTrip clientToTrip);
        Task CheckPesel(ClientToTrip clientToTrip);
        Task <bool> CheckIfTripExists(ClientToTrip clientToTrip);
        Task<bool> CheckIfClientIsRegistered(ClientToTrip clientToTrip);
    }
}
