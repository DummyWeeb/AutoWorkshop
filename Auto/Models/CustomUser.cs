using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Auto.Models
{
    public class CustomUser : IdentityUser
    {
        [Required(ErrorMessage = "Фамилия обязательна.")]
        [StringLength(255, ErrorMessage = "Фамилия не должна превышать 255 символов.")]
        [Display(Name = "Фамилия")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Имя обязательно.")]
        [StringLength(255, ErrorMessage = "Имя не должно превышать 255 символов.")]
        [Display(Name = "Имя")]
        public string? Ima { get; set; }

        [StringLength(255, ErrorMessage = "Отчество не должно превышать 255 символов.")]
        [Display(Name = "Отчество")]
        public string? SecSurname { get; set; }

        [Required(ErrorMessage = "Подразделение обязательно.")]
        [Display(Name = "Подразделение")]
        public int PodrazdelenieId { get; set; }

        public virtual Podrazdelenie Podrazdelenie { get; set; }
    }
}