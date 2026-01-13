using Microsoft.EntityFrameworkCore;

namespace PetSociety.API.Models
{
    public partial class PetSocietyContext : DbContext
    {
        // 空白建構子
        public PetSocietyContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                // 取得 ASP.NET Core 的環境名稱 (Development/Production/Staging)
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                   ?? "Production";  // fallback

                IConfiguration Config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)  // 支援 Development.json
                    .Build();
                var conn = Config.GetConnectionString("PetSocietyContext");

                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException("找不到 ConnectionStrings:PetSocietyContext");

                optionsBuilder.UseSqlServer(Config.GetConnectionString("PetSocietyContext"));
            }
        }
    }
}
