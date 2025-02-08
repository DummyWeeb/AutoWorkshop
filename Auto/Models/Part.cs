namespace Auto.Models
{
    public class Part
    {
        public int PartId { get; set; } // Идентификатор запчасти
        public string Name { get; set; } // Название запчасти
        public int ModelId { get; set; } // Внешний ключ к модели
        public virtual Model Model { get; set; } // Связь с моделью
    }


}
