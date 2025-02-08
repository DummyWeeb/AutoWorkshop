namespace Auto.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; } // Идентификатор поставщика
        public string Name { get; set; } // Название поставщика
        public string ContactInfo { get; set; } // Контактная информация
        public virtual ICollection<Part> Parts { get; set; } // Связанные запчасти
    }


}
