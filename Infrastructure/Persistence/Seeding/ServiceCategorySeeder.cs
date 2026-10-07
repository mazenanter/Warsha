using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeding
{
    public static class ServiceCategorySeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.ServiceCategories.AnyAsync())
                return;

            var categories = new List<ServiceCategory>
        {
            ServiceCategory.Create(
                "Engine",
                "المحرك",
                "engine"),

            ServiceCategory.Create(
                "Transmission",
                "ناقل الحركة",
                "transmission"),

            ServiceCategory.Create(
                "Oils & Fluids",
                "الزيوت والسوائل",
                "oil"),

            ServiceCategory.Create(
                "Brakes",
                "الفرامل",
                "brake"),

            ServiceCategory.Create(
                "Suspension",
                "العفشة",
                "suspension"),

            ServiceCategory.Create(
                "Tires & Wheels",
                "الإطارات والعجلات",
                "tire"),

            ServiceCategory.Create(
                "Auto Electrical",
                "كهرباء السيارات",
                "electrical"),

            ServiceCategory.Create(
                "Air Conditioning",
                "تكييف السيارات",
                "ac"),

            ServiceCategory.Create(
                "Cooling System",
                "نظام التبريد",
                "cooling"),

            ServiceCategory.Create(
                "Fuel System",
                "نظام الوقود",
                "fuel"),

            ServiceCategory.Create(
                "Exhaust System",
                "العادم",
                "exhaust"),

            ServiceCategory.Create(
                "Diagnostics",
                "تشخيص الأعطال",
                "diagnostics"),

            ServiceCategory.Create(
                "Batteries",
                "البطاريات",
                "battery"),

            ServiceCategory.Create(
                "Body Repair",
                "سمكرة",
                "body"),

            ServiceCategory.Create(
                "Paint",
                "دهان",
                "paint"),

            ServiceCategory.Create(
                "Auto Glass",
                "زجاج السيارات",
                "glass"),

            ServiceCategory.Create(
                "Lighting",
                "الإضاءة",
                "lighting"),

            ServiceCategory.Create(
                "Car Detailing",
                "تلميع وعناية",
                "detailing"),

            ServiceCategory.Create(
                "Car Wash",
                "غسيل السيارات",
                "car-wash"),

            ServiceCategory.Create(
                "Interior Services",
                "العناية بالصالون",
                "interior"),

            ServiceCategory.Create(
                "Periodic Maintenance",
                "الصيانة الدورية",
                "maintenance"),

            ServiceCategory.Create(

                "Vehicle Inspection",
                "فحص السيارات",
                "inspection"),

            ServiceCategory.Create(

                "Calibration",
                "المعايرة",
                "calibration"),

            ServiceCategory.Create(

                "Keys & Security",
                "المفاتيح وأنظمة الأمان",
                "key"),

            ServiceCategory.Create(

                "Car Accessories",
                "إكسسوارات السيارات",
                "accessories"),

            ServiceCategory.Create(

                "Performance",
                "تعديل وتحسين الأداء",
                "performance"),

            ServiceCategory.Create(

                "Hybrid & EV",
                "السيارات الهجينة والكهربائية",
                "ev"),

            ServiceCategory.Create(


                "Underbody",
                 "فحص وإصلاح أسفل السيارة",
                "underbody"),

            ServiceCategory.Create(


                "Roadside Assistance",
                 "المساعدة على الطريق",
                "roadside")
        };

            await context.ServiceCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}
