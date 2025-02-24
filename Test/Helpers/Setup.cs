using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MinimalApi.Dominio.Interfaces;
using Test.Mocks;
using Microsoft.Extensions.DependencyInjection;


namespace Api.Test.Helpers;

public class Setup
{
    public const string PORT = "5001";
        
    public static TestContext testContext = default!;
        
    public static WebApplicationFactory<Startup> http = default!;
        
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
                /*
                // == Caso queira deixar o teste com conexão diferente ==
                var conexao = "Server=localhost;Database=MinimalApiTeste;Uid=root;";
                services.AddDbContext<DbContexto>(options =>
                {
                    options.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
                });
                */
            });
        });

        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}