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
        public int? Year { get; set; } // Год выпуска модели (опционально)

        [Display(Name = "Логотип")]
        public string? LogoPath { get; set; } // Путь к логотипу

        // Связи
        public int BrandId { get; set; } // Внешний ключ к марке
        public Brand? Brand { get; set; } // Навигационное свойство для марки
        public virtual ICollection<Part>? Parts { get; set; } // Many-to-many связь с запчастями
    }
}