using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Auto.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        [Required(ErrorMessage = "Количество обязательно.")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0.")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Цена за единицу")]
        public decimal PricePerUnit { get; set; } // Цена за единицу в момент заказа

        // Связи
        public int OrderId { get; set; } // Внешний ключ к заказу
        public Order? Order { get; set; } // Навигационное свойство для заказа

        public int PartId { get; set; } // Внешний ключ к запчасти
        public Part? Part { get; set; } // Навигационное свойство для запчасти
    }
}
