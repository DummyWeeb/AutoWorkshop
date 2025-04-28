using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Auto.Models
{
    public class CarModel
    {
        public int CarModelId { get; set; }

        [Required(ErrorMessage = "Название модели обязательно.")]
        [StringLength(255, ErrorMessage = "Название не должно превышать 255 символов.")]
        [Display(Name = "Модель")]
        public string? Name { get; set; }

        [Display(Name = "Год выпуска")]
        public int? Year { get; set; } 

        [Display(Name = "Логотип")]
        public string? LogoPath { get; set; } 

        // Связи
        public int BrandId { get; set; } 
        public Brand? Brand { get; set; } 
        public virtual ICollection<Part>? Parts { get; set; } 
    }
}

