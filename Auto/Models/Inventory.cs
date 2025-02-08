namespace Auto.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; } // Идентификатор склада
        public int PartId { get; set; } // Внешний ключ к запчасти
        public int Quantity { get; set; } // Количество запчастей на складе
        public virtual Part Part { get; set; } // Связь с запчастью
    }


}
