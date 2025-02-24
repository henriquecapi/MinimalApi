using Microsoft.AspNetCore.WebUtilities;
using Api.Test.Helpers;
using MinimalApi.Dominio.DTOs;
using System.Text.Json;
using System.Text;
using MinimalApi.Dominio.ModelViews;
using System.Net;

namespace Test.Requests
{
    public class AdministradorRequestTest
    {
        [ClassInitialize]  // Quando estiver começando esse teste
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }
        // ao terminar os testes vai limpar
        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }
        [TestMethod]
        public async Task TestarGetSetPropriedades()
        {
            // Arrange  - São todas as variáveis para as validações
            var loginDTO = new LoginDTO{
                Email = "admin@teste.com",
                Senha = "admin123"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            // Act - São as ações dos itens (set de  algumas propriedades)
            var response = await Setup.client.PostAsync("/administradores/login", content);

            // Assert - Validação desses dados (Get)
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            //var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions());
            
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.IsNotNull(admLogado?.Email ?? "");
            Assert.IsNotNull(admLogado?.Token ?? "");
            Assert.IsNotNull(admLogado?.Perfil ?? "");

            Console.WriteLine(admLogado?.Token);
        }
    }
}
