using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Auto.Models
{
    public class Brand
    {
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Название марки обязательно.")]
        [StringLength(255, ErrorMessage = "Название не должно превышать 255 символов.")]
        [Display(Name = "Название марки")]
        public string Name { get; set; }

        public virtual ICollection<CarModel>? Models { get; set; } // Список моделей
    }
}
