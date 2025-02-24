
using Api;
using MinimalApi;

internal class Program
{
    private static void Main(string[] args)
    {
        IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
        CreateHostBuilder(args).Build().Run();
    }
}