
namespace Order.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var app = builder.Build();


            app.MapGet("/ready", () =>
            {
                Console.WriteLine("Order service is ready");
                return true;
            });

            app.MapGet("/commit", () =>
            {
                Console.WriteLine("Order service is commited");
                return true;
            });
         
            app.MapGet("/rollback", () =>
            {
                Console.WriteLine("Order service is rollbacked");
            });

            app.Run();
        }
    }
}
