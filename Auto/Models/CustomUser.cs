using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Auto.Models
{
    public class CustomUser : IdentityUser
    {
        [Required(ErrorMessage = "Фамилия обязательна.")]
        [StringLength(255, ErrorMessage = "Фамилия не должна превышать 255 символов.")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Имя обязательно.")]
        [StringLength(255, ErrorMessage = "Имя не должно превышать 255 символов.")]
        public string? Ima { get; set; }

        [StringLength(255, ErrorMessage = "Отчество не должно превышать 255 символов.")]
        public string? SecSurname { get; set; }

        [Required(ErrorMessage = "Возраст обязателен.")]
        [Range(18, 100, ErrorMessage = "Возраст должен быть от 18 до 100 лет.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Подразделение обязательно.")]
        public int PodrazdelenieId { get; set; }

        public virtual Podrazdelenie Podrazdelenie { get; set; }
    }
}