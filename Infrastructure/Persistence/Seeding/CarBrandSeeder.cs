using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeding
{
    public static class CarBrandSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.CarBrands.AnyAsync()) return;

            var brands = new[]
            {
            CarBrand.Create("Toyota"),
            CarBrand.Create("BMW"),
            CarBrand.Create("Mercedes"),
            CarBrand.Create("Volkswagen"),
            CarBrand.Create("Hyundai"),
            CarBrand.Create("Kia"),
            CarBrand.Create("Chevrolet"),
            CarBrand.Create("Ford"),
            CarBrand.Create("Nissan"),
            CarBrand.Create("Honda"),
            CarBrand.Create("Mitsubishi"),
            CarBrand.Create("Peugeot"),
            CarBrand.Create("Opel"),
            CarBrand.Create("Fiat"),
            CarBrand.Create("Suzuki"),
            CarBrand.Create("Skoda"),
            CarBrand.Create("Jeep"),
            CarBrand.Create("Land Rover"),
            CarBrand.Create("Audi"),
            CarBrand.Create("Renault"),
            CarBrand.Create("Mazda"),
            CarBrand.Create("Subaru"),
            CarBrand.Create("Volvo"),
            CarBrand.Create("Porsche"),
            CarBrand.Create("Lexus"),
        };

            await context.CarBrands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }
    }
}
