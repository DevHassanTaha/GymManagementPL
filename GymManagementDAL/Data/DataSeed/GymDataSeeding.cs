using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.DataSeed
{
    public class GymDataSeeding
    {
        public static bool SeedData(GymDbContext context)
        {
            try
            {

                if (!context.categories.Any())
                {
                    var categories = LoadDataFromJsonFile<Category>("categories.json");
                    context.categories.AddRange(categories);

                }
                if (!context.Plans.Any())
                {
                    var plans = LoadDataFromJsonFile<Plan>("plans.json");
                    context.Plans.AddRange(plans);
                }
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Data seeding failed: ");
            }

        }
        private static List<T> LoadDataFromJsonFile<T>(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\Files", fileName);

            if(!File.Exists(filePath))
                throw new FileNotFoundException("Data file not found: " + filePath);

            var jsonData = File.ReadAllText(filePath);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<List<T>>(jsonData, options);

        }
    }
}
