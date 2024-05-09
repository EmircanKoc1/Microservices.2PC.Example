using Coordinator.Models.Contexts;
using Coordinator.Services;
using Coordinator.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coordinator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


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


            builder.Services.AddTransient<ITransactionService, TransactionService>();



            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.MapGet("/create-order-transaction", async ([FromServices] ITransactionService transactionService) =>
            {
                //phase1
                var transactionId = await transactionService.CreateTransactionAsync();

                await transactionService.PrepareServicesAsync(transactionId);

                bool transactionState = await transactionService.CheckTransactionStateServiceAsync(transactionId);

                if (transactionState)
                {
                    await transactionService.CommitAsync(transactionId);
                    transactionState = await transactionService.CheckTransactionStateServiceAsync(transactionId);

                }

                if (!transactionState)
                    await transactionService.RollbackAsync(transactionId);

            });







            app.Run();
        }
    }
}
