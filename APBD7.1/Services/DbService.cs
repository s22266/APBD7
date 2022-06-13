using System.Collections.Generic;
using System.Threading.Tasks;
using APBD7._1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using APBD7._1.Models.DTO;

namespace APBD7._1.Services
{
    public class DbService : IDbService
    {
        private readonly masterContext _dbContext;
        public DbService(masterContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<object>> GetTrips()
        {
            /*throw new System.NotImplementedException();*/
            return await _dbContext.Trips
                .Include(e => e.CountryTrips)
                .Include(e => e.ClientTrips)
                .Select(e => new SomeSortOfTrip
                {
                    Name = e.Name,
                    Description = e.Description,
                    MaxPeople = e.MaxPeople,
                    DateFrom = e.DateFrom,
                    DateTo = e.DateTo,
                    Countries = e.CountryTrips.Select(e => new SomeSortOfCountry { Name = e.IdCountryNavigation.Name}).ToList(),
                    Clients = e.ClientTrips.Select(e => new SomeSortOfClient { FirstName = e.IdClientNavigation.FirstName, LastName = e.IdClientNavigation.LastName}).ToList()
                }).OrderByDescending(e => e.DateFrom).ToListAsync();
        }

        public async Task<bool> DeleteClient(int idClient)
        {
            var clientTrip = await _dbContext.ClientTrips.AnyAsync(e => e.IdClient == idClient);

            if (clientTrip)
            {
                return false;
            }
            else 
            {
                var client = new Client()
                {
                    IdClient = idClient
                };

                _dbContext.Attach(client);
                _dbContext.Remove(client);
                await _dbContext.SaveChangesAsync();

                return true;
            }
        }

        public async Task CheckPesel(ClientToTrip clientToTrip) 
        {
            var pesel = await _dbContext.Clients.AnyAsync(e => e.Pesel.Equals(clientToTrip.Pesel));
            if (!pesel) 
            {
                var client = new Client
                {
                    FirstName = clientToTrip.FirstName,
                    LastName = clientToTrip.LastName,
                    Email = clientToTrip.Email,
                    Telephone = clientToTrip.Telephone,
                    Pesel = clientToTrip.Pesel
                };
                _dbContext.Add(client);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckIfTripExists(ClientToTrip clientToTrip) 
        {
            var trip = await _dbContext.ClientTrips.AnyAsync(e => e.IdTrip == clientToTrip.IdTrip);
            return trip;
        }

        public Task<bool> CheckIfClientIsRegistered(ClientToTrip clientToTrip) 
        {
            var registered = _dbContext.ClientTrips.AnyAsync(e =>
                (e.IdClient == _dbContext.Clients.Where(c => c.Pesel.Equals(clientToTrip.Pesel))
                                                 .Select(s => s.IdClient).First())
                &&
                (e.IdTrip == clientToTrip.IdTrip));

            return registered;
        }

        public async Task AddClientToTrip(ClientToTrip clientToTrip)
        {
            var client = await _dbContext.Clients.Where(c => c.Pesel.Equals(clientToTrip.Pesel)).FirstOrDefaultAsync();
            var trip = await _dbContext.Trips.Where(t => t.IdTrip == clientToTrip.IdTrip).FirstOrDefaultAsync();

            var clientTrip = new ClientTrip
            {
                IdClientNavigation = client,
                IdTripNavigation = trip,
                RegisteredAt = System.DateTime.Now,
                PaymentDate = System.DateTime.Now
            };

            _dbContext.Add(clientTrip);
            await _dbContext.SaveChangesAsync();
        }

    }
}
