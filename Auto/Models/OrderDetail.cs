namespace Auto.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; } // Идентификатор детали заказа
        public int OrderId { get; set; } // Внешний ключ к заказу
        public int PartId { get; set; } // Внешний ключ к запчасти
        public int Quantity { get; set; } // Количество запчастей в заказе
        public virtual Order Order { get; set; } // Связь с заказом
        public virtual Part Part { get; set; } // Связь с запчастью
    }


}
