using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SportsRaffles.Data;
using SportsRaffles.Data.Models;
using SportsRaffles.ViewModels;

namespace SportsRaffles.Services
{
    public class CompetitionService
    {
        private readonly AppDbContext _context;

        public CompetitionService(AppDbContext context)
        {
            _context = context;
        }

        #region Get Methods

        public async Task<Competition> GetCompetitionAsync(Guid id)
        {
            var competition = await _context.Competitions
                .FirstOrDefaultAsync(x => x.Id == id);

            return competition;
        }

        public async Task<List<Competition>> GetCompetitionsAsync()
        {
            var competitions = await _context.Competitions
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .ToListAsync();

            return competitions;
        }


        public async Task<CompetitionViewModel> GetCompetitionViewModel(Guid id)
        {
            var prizes = await _context.Prizes
                .Where(x => x.CompetitionId == id)
                .Include(x => x.Competition)
                .ToListAsync();

            if (prizes == null)
            {
                //Log error
            }

            var competition = prizes.FirstOrDefault().Competition;
            var competitionModel = new CompetitionViewModel
            {
                Id = competition.Id,
                Title = competition.Name,
                Description = competition.Description,
                MaxTickets = competition.MaxTickets,
                MinTickets = competition.MinTickets,
                MaxTicketsPerUser = competition.MaxEntriesPerUser,
                CostPerTicket = competition.TicketCost,
                StartDate = competition.StartDate,
                EndDate = competition.EndDate,
                Prizes = prizes.Select(x => new PrizeViewModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    Cost = x.Cost,
                    ImageUrl = x.ImageUrl
                }).ToList()
            };

            return competitionModel;
        }

        public async Task<List<CompetitionViewModel>> GetIndexCompetitions()
        {

            var competitionIds = await _context.Competitions
                .OrderByDescending(x => x.EndDate)
                .Take(8)
                .Select(x => x.Id)
                .ToListAsync();

            var prizes = await _context.Prizes
                .Where(x => competitionIds.Contains(x.CompetitionId))
                .Include(x => x.Competition)
                .ToListAsync();

            var competitions = new List<CompetitionViewModel>();

            foreach (var compId in competitionIds)
            {
                var prizesList = prizes.Where(x => x.CompetitionId == compId);
                var competition = prizesList.FirstOrDefault().Competition;
                competitions.Add(new CompetitionViewModel
                {
                    Id = competition.Id,
                    Title = competition.Name,
                    Description = competition.Description,
                    MaxTickets = competition.MaxTickets,
                    MinTickets = competition.MinTickets,
                    MaxTicketsPerUser = competition.MaxEntriesPerUser,
                    CostPerTicket = competition.TicketCost,
                    StartDate = competition.StartDate,
                    EndDate = competition.EndDate,
                    Prizes = prizesList.Select(x => new PrizeViewModel
                    {
                        Name = x.Name,
                        Description = x.Description,
                        Cost = x.Cost,
                        ImageUrl = x.ImageUrl
                    }).ToList()
                });
            }

            return competitions;
        }

        public int GetEntriesForCompetition(Guid id) => _context.Ticket.Where(x => x.CompetitionId == id).Count();
        

        #endregion

        #region CRUD Methods

        public async Task CreateCompetition(CompetitionViewModel competitionViewModel)
        {
            if (competitionViewModel == null)
            {
                //Log error
                return;
            }

            var prizes = competitionViewModel.Prizes.Select(x =>
                    new Prize(x.Name, x.Description, x.Cost, x.ImageUrl)).ToList();

            var competition = new Competition(competitionViewModel.Title,
                competitionViewModel.Description,
                competitionViewModel.MaxTickets,
                competitionViewModel.MinTickets,
                competitionViewModel.MaxTicketsPerUser,
                competitionViewModel.CostPerTicket,
                competitionViewModel.StartDate.Value,
                competitionViewModel.EndDate.Value);

            prizes.ForEach(x => competition.Prizes.Add(x));
            await _context.Competitions.AddAsync(competition);
            await _context.Prizes.AddRangeAsync(prizes);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCompetition(Guid id)
        {
            var competition = await _context.Competitions
              .FirstOrDefaultAsync(x => x.Id == id);

            if (competition == null)
            {
                //Log error
                return false;
            }

            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task EditCompetition(Guid Id, CompetitionViewModel competitionViewModel)
        {
            if (competitionViewModel == null)
            {
                //Log error
                return;
            }

            var competition = await _context.Competitions
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (competition != null)
            {
                var prizes = competitionViewModel.Prizes.Select(x =>
                             new Prize(x.Name, x.Description, x.Cost, x.ImageUrl)).ToList();

                competition.Prizes = prizes;
                competition.Name = competitionViewModel.Title;
                competition.Description = competitionViewModel.Description;
                competition.MaxTickets = competitionViewModel.MaxTickets;
                competition.MinTickets = competitionViewModel.MinTickets;
                competition.MaxEntriesPerUser = competitionViewModel.MaxTicketsPerUser;
                competition.TicketCost = competitionViewModel.CostPerTicket;
                competition.StartDate = competitionViewModel.StartDate.Value;
                competition.EndDate = competitionViewModel.EndDate.Value;

                _context.Update(competition);
                _context.UpdateRange(prizes);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
