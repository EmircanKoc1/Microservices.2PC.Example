
namespace Payment.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            
            var app = builder.Build();


            app.MapGet("/ready", () =>
            {
                Console.WriteLine("payment service is ready");
                return true;
            });

            app.MapGet("/commit", () =>
            {
                Console.WriteLine("payment service is commited");
                return true;
            });
         
            app.MapGet("/rollback", () =>
            {
                Console.WriteLine("payment service is rollbacked");
            });

            app.Run();
        }
    }
}
