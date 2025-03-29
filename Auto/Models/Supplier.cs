using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Auto.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Название поставщика обязательно.")]
        [StringLength(255, ErrorMessage = "Название не должно превышать 255 символов.")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Адрес не должен превышать 255 символов.")]
        [Display(Name = "Адрес")]
        public string? Address { get; set; } // Опциональный адрес

        [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов.")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; } // Опциональный телефон

        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        [Display(Name = "Email")]
        public string? Email { get; set; } // Опциональный email

        // Связи
        public virtual ICollection<Part>? Parts { get; set; } // Связь с запчастями
    }
}