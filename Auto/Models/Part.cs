using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Auto.Models
{
    public class Part
    {
        public int PartId { get; set; }

        [Required(ErrorMessage = "Название запчасти обязательно.")]
        [StringLength(255, ErrorMessage = "Название не должно превышать 255 символов.")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; } // Опциональное описание

        [Required(ErrorMessage = "Количество обязательно.")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество должно быть неотрицательным.")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        // Связи
        public virtual ICollection<CarModel>? CarModels { get; set; } // Добавлена коллекция CarModels
    }
}