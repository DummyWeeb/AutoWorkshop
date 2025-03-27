using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Auto.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Дата заказа обязательна.")]
        [Display(Name = "Дата заказа")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Общая сумма")]
        public decimal TotalAmount { get; set; } // Вычисляемое поле

        [Display(Name = "Статус")]
        public string? Status { get; set; }

        // Связи
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } // Связь с деталями заказа
    }
}
