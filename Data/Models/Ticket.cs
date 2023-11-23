using Microsoft.EntityFrameworkCore;

namespace SportsRaffles.Data.Models
{
    [PrimaryKey(nameof(Id))]
    public class Ticket
    {
        public Ticket() { }

        public Ticket(float cost, Competition competition)
        {
            Id = Guid.NewGuid();
            Competition = competition;
            CompetitionId = competition.Id;
        }

        public Guid Id { get; }

        public Competition Competition { get; set; }
        public Guid CompetitionId { get; set; }

    }
}
