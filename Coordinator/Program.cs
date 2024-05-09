using Coordinator.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Coordinator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });


            builder.Services.AddHttpClient("OrderAPI", config =>
            {
                config.BaseAddress = new Uri("https://localhost:7196/");

            });

            builder.Services.AddHttpClient("StockAPI", config =>
            {
                config.BaseAddress = new Uri("https://localhost:7017/");

            });

            builder.Services.AddHttpClient("PaymentAPI", config =>
            {
               config.BaseAddress = new Uri("https://localhost:7143/");

            });




            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
