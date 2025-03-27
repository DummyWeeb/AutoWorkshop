namespace Auto.Models
{
    public class Podrazdelenie
    {
        public int PodrazdelenieId { get; set; }
        public string? PodrazdelenieName { get; set; }
        public virtual ICollection<CustomUser> CustomUser { get; set; } 
    }
}
