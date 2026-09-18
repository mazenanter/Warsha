using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeding
{
    public static class CarModelSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.CarModels.AnyAsync()) return;

            var brands = await context.CarBrands
                .ToDictionaryAsync(b => b.Name, b => b.Id);

            var data = new Dictionary<string, string[]>
            {
                ["Toyota"] =
                [
                    "Corolla", "Camry", "Yaris", "Land Cruiser",
                "RAV4", "Highlander", "Fortuner", "Hilux",
                "Prado", "Avalon", "C-HR", "Rush"
                ],
                ["BMW"] =
                [
                    "316i", "318i", "320i", "328i", "330i",
                "520i", "523i", "528i", "530i", "730i", "740i",
                "X1", "X3", "X5", "X6", "M3", "M5", "Z4"
                ],
                ["Mercedes"] =
                [
                    "A180", "A200", "C180", "C200", "C250",
                "E180", "E200", "E250", "E300",
                "S400", "S500", "GLA", "GLC", "GLE", "GLS",
                "CLA", "CLS", "Vito", "Sprinter"
                ],
                ["Volkswagen"] =
                [
                    "Golf", "Passat", "Polo", "Tiguan",
                "Touareg", "Jetta", "T-Roc", "Arteon", "ID.4"
                ],
                ["Hyundai"] =
                [
                    "Elantra", "Sonata", "Tucson", "Santa Fe",
                "Accent", "i10", "i20", "i30",
                "Creta", "Kona", "Palisade", "Venue"
                ],
                ["Kia"] =
                [
                    "Sportage", "Sorento", "Cerato", "Optima",
                "Picanto", "Rio", "Stinger", "Telluride",
                "K5", "K8", "Seltos", "Carnival"
                ],
                ["Chevrolet"] =
                [
                    "Optra", "Cruze", "Malibu", "Aveo",
                "Captiva", "Traverse", "Blazer", "Tahoe",
                "Suburban", "Trax", "Lanos", "Spark"
                ],
                ["Ford"] =
                [
                    "Focus", "Fiesta", "Mondeo", "Fusion",
                "Escape", "Explorer", "Edge", "Expedition",
                "Mustang", "F-150", "EcoSport", "Puma"
                ],
                ["Nissan"] =
                [
                    "Sunny", "Sentra", "Altima", "Maxima",
                "X-Trail", "Qashqai", "Pathfinder", "Patrol",
                "Juke", "Kicks", "Navara", "Micra"
                ],
                ["Honda"] =
                [
                    "Civic", "Accord", "City", "Jazz",
                "CR-V", "HR-V", "Pilot", "Odyssey",
                "BR-V", "ZR-V"
                ],
                ["Mitsubishi"] =
                [
                    "Lancer", "Galant", "Eclipse", "Outlander",
                "ASX", "Pajero", "L200", "Attrage",
                "Xpander", "Eclipse Cross"
                ],
                ["Peugeot"] =
                [
                    "206", "207", "208", "301",
                "306", "307", "308", "405",
                "408", "508", "2008", "3008", "5008"
                ],
                ["Opel"] =
                [
                    "Astra", "Vectra", "Corsa", "Insignia",
                "Mokka", "Grandland", "Crossland", "Zafira"
                ],
                ["Fiat"] =
                [
                    "Punto", "Tipo", "500",
                "Bravo", "Doblo", "Freemont"
                ],
                ["Suzuki"] =
                [
                    "Swift", "Vitara", "S-Cross", "Jimny",
                "Baleno", "Celerio", "Ertiga", "XL7"
                ],
                ["Skoda"] =
                [
                    "Octavia", "Superb", "Fabia",
                "Kodiaq", "Karoq", "Kamiq", "Scala"
                ],
                ["Jeep"] =
                [
                    "Wrangler", "Cherokee", "Grand Cherokee",
                "Compass", "Renegade", "Gladiator"
                ],
                ["Land Rover"] =
                [
                    "Discovery", "Discovery Sport", "Defender",
                "Range Rover", "Range Rover Sport",
                "Range Rover Evoque", "Range Rover Velar"
                ],
                ["Audi"] =
                [
                    "A3", "A4", "A5", "A6", "A7", "A8",
                "Q3", "Q5", "Q7", "Q8",
                "TT", "R8", "e-tron"
                ],
                ["Renault"] =
                [
                    "Clio", "Megane", "Logan", "Symbol",
                "Duster", "Captur", "Koleos",
                "Fluence", "Kadjar", "Arkana"
                ],
                ["Mazda"] =
                [
                    "Mazda 2", "Mazda 3", "Mazda 6",
                "CX-3", "CX-5", "CX-9", "MX-5"
                ],
                ["Subaru"] =
                [
                    "Impreza", "Legacy", "Outback",
                "Forester", "XV", "WRX", "BRZ"
                ],
                ["Volvo"] =
                [
                    "S60", "S90", "V60", "V90",
                "XC40", "XC60", "XC90", "C40"
                ],
                ["Porsche"] =
                [
                    "911", "Cayenne", "Macan",
                "Panamera", "Taycan", "718"
                ],
                ["Lexus"] =
                [
                    "IS", "ES", "GS", "LS",
                "NX", "RX", "GX", "LX",
                "UX", "RC", "LC"
                ],
            };

            var models = new List<CarModel>();

            foreach (var (brandName, modelNames) in data)
            {
                if (!brands.TryGetValue(brandName, out var brandId))
                    continue;

                foreach (var modelName in modelNames)
                    models.Add(CarModel.Create(modelName, brandId));
            }

            await context.CarModels.AddRangeAsync(models);
            await context.SaveChangesAsync();
        }
    }
}
