using SportsRaffles.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SportsRaffles.ViewModels
{
    public class CompetitionViewModel
    {
        public Guid? Id { get; set; }
        [Required]public string Title { get; set; }
        [Required]public string Description { get; set; }
        [Required]public int MaxTickets { get; set; }
        [Required]public int MinTickets { get; set; }
        [Required]public int MaxTicketsPerUser { get; set; }
        [Required]public float CostPerTicket { get; set; }
        [Required]public DateTime? StartDate { get; set; }
        [Required]public DateTime? EndDate { get; set; }
        public List<PrizeViewModel> Prizes { get; set; } = new List<PrizeViewModel>();
    }

    public class PrizeViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public string ImageUrl { get; set; }
    }
}
