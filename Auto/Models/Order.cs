namespace Auto.Models
{
    public class Order
    {
        public int OrderId { get; set; } // Идентификатор заказа
        public int SupplierId { get; set; } // Внешний ключ к поставщику
        public DateTime OrderDate { get; set; } // Дата заказа
        public virtual Supplier Supplier { get; set; } // Связь с поставщиком
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } // Детали заказа
    }


}
