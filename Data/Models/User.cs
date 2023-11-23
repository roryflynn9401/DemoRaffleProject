using Microsoft.AspNetCore.Identity;

namespace SportsRaffles.Data.Models
{
    public class User : IdentityUser<Guid>
    {
        public User() : base()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
            Verified = false;
            DisplayName = UserName;

            //Default Country
            Country = "Unknown";
        }

        public DateTime Created { get;  }
        public DateTime? Modified { get; set; }
        public string? DisplayName { get; set; }
        public bool Verified { get; set; }
        public string? Country { get; set; }
        public string? AvatarUri { get; set; }

        public ICollection<Competition> Competitions { get; set; } = new List<Competition>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
