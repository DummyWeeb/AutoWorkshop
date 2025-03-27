using Microsoft.AspNetCore.Identity;

namespace Auto.Models
{
    public class CustomUser:IdentityUser
    {
        public string? Surname { get; set; }

        public string? Ima { get; set; }

        public string? SecSurname { get; set; }
        public int Age { get; set; }
        public int PodrazdelenieId { get; set; }
        public virtual Podrazdelenie Podrazdelenie { get; set; }
    }
}
