using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

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

        [Required(ErrorMessage = "Цена обязательна.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0.01.")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Display(Name = "Изображение")]
        public string? ImageUrl { get; set; } // URL изображения запчасти

        // Связи
        public int SupplierId { get; set; } // Внешний ключ к поставщику
        public Supplier? Supplier { get; set; } // Навигационное свойство для поставщика
        public virtual ICollection<CarModel>? CarModels { get; set; }
        public virtual ICollection<Inventory>? Inventories { get; set; } // Связь с запасами
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } // Связь с деталями заказа
    }
}
