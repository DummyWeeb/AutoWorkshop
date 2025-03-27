using Auto.Data;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Auto.Models;

namespace Auto.Data
{
    public class InitData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                Podrazdelenie  podrazdelenie1 = new ()
                {
                    PodrazdelenieName = "Бухгалтерия"
                    
                };

                context.SaveChanges();

                Podrazdelenie podrazdelenie2 = new()
                {
                    PodrazdelenieName = "Кладовщики"

                };
                context.Podrazdelenies.Add(podrazdelenie2);
                context.SaveChanges();

                Podrazdelenie podrazdelenie3 = new()
                {
                    PodrazdelenieName = "ИТ"

                };

                context.Podrazdelenies.Add(podrazdelenie3);
                context.SaveChanges();
                Podrazdelenie podrazdelenie4 = new()
                {
                    PodrazdelenieName = "Администрация"

                };
                context.Podrazdelenies.Add(podrazdelenie4);
                context.SaveChanges();

                
                context.SaveChanges();
                await context.SaveChangesAsync(); ;

            }
        }
    }

}

