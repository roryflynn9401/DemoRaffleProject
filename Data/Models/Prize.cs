using Microsoft.EntityFrameworkCore;

namespace SportsRaffles.Data.Models
{
    [PrimaryKey(nameof(Id))]
    public class Prize
    {
        public Prize() { }

        public Prize(string name, string desc, float cost, string image) 
        {
            if(cost == 0) throw new Exception("Cost cannot be 0");
            Id = Guid.NewGuid();
            Name = name;
            Description = desc;
            Cost = cost;
            ImageUrl = image;
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public string ImageUrl { get; set; }
        public PrizeStatus Status { get; set; }

        public Competition Competition { get; set; }
        public Guid CompetitionId { get; set; }

    }
}
