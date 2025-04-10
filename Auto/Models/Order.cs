using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Auto.Data;

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
        public decimal TotalAmount { get; set; }

        [Display(Name = "Статус")]
        public OrderStatus Status { get; set; } = OrderStatus.Заказано;

        [Display(Name = "Номер заказа")]
        public int OrderNumber => OrderId; // Используем OrderId в качестве номера заказа

        // Связи
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Display(Name = "Название поставщика")]
        public string? SupplierName { get; set; }

        public virtual ICollection<OrderPart>? OrderParts { get; set; }
    }
}