using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace SportsRaffles.Data.Models
{
    [PrimaryKey(nameof(Id))]
    public class Competition
    {
        public Competition() { Tickets = new Collection<Ticket>(); Status = CompetitionStatus.Pending; }

        public Competition(string name, string description, int maxTickets, int minTickets, int maxEntriesPerUser,float ticketCost, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            MaxTickets = maxTickets;
            MinTickets = minTickets;
            MaxEntriesPerUser = maxEntriesPerUser;
            TicketCost = ticketCost;
            Status = CompetitionStatus.Pending;
            StartDate = startDate;
            EndDate = endDate;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set;}

        public int MaxTickets { get; set; }
        public int MinTickets { get; set; }
        public int MaxEntriesPerUser { get; set; }
        public float TicketCost { get; set; }
        public CompetitionStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public User? Winner { get; set; }
        public Guid? WinnerId { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Prize> Prizes { get; set; } = new List<Prize>();
    }
}
