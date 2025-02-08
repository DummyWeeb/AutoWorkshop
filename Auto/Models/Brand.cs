namespace Auto.Models
{
    public class Brand
    {
        public int BrandId { get; set; } // Идентификатор марки
        public string Name { get; set; } // Название марки
        public virtual ICollection<Model> Models { get; set; } // Связанные модели
    }


}
