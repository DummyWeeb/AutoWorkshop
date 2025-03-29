using System.ComponentModel.DataAnnotations;

namespace Auto.Data
{
    public enum OrderStatus
    {
        [Display(Name = "Заказано")]
        Заказано,

        [Display(Name = "Заказ выполнен")]
        ЗаказВыполнен
    }
}