namespace Auto.Models
{
    public class Model
    {
        public int ModelId { get; set; } // Идентификатор модели
        public string Name { get; set; } // Название модели
        public int BrandId { get; set; } // Внешний ключ к марке
        public virtual Brand Brand { get; set; } // Связь с маркой
        public virtual ICollection<Part> Parts { get; set; } // Связанные запчасти
    }


}
