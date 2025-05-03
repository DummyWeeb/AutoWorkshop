using Auto.Data;
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
               
                if (!context.Podrazdelenies.Any(p => p.PodrazdelenieName == "Отдел закупок"))
                {
                    Podrazdelenie podrazdelenie1 = new()
                    {
                        PodrazdelenieName = "Отдел закупок"
                    };
                    context.Podrazdelenies.Add(podrazdelenie1);
                }

                if (!context.Podrazdelenies.Any(p => p.PodrazdelenieName == "Кладовщики"))
                {
                    Podrazdelenie podrazdelenie2 = new()
                    {
                        PodrazdelenieName = "Кладовщики"
                    };
                    context.Podrazdelenies.Add(podrazdelenie2);
                }

                if (!context.Podrazdelenies.Any(p => p.PodrazdelenieName == "ИТ"))
                {
                    Podrazdelenie podrazdelenie3 = new()
                    {
                        PodrazdelenieName = "ИТ"
                    };
                    context.Podrazdelenies.Add(podrazdelenie3);
                }

                if (!context.Podrazdelenies.Any(p => p.PodrazdelenieName == "Администрация"))
                {
                    Podrazdelenie podrazdelenie4 = new()
                    {
                        PodrazdelenieName = "Администрация"
                    };
                    context.Podrazdelenies.Add(podrazdelenie4);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}