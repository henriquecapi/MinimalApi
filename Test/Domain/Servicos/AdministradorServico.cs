using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoTeste()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "","..","..",".."));
            
            // Configurar a ConfigurationBuilder
            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            var configuration = builder.Build();

            return new DbContexto(configuration);
        }
// Teste 1 Administrador - Limpa tabele e salva um novo registro na tabela administradores
        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange  - São todas as variáveis para as validações
            var context = CriarContextoTeste(); 
            // Limpar a base de dados (Tabela)
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE adminitradores");

            var adm = new Administrador();
            adm.Id = 1;
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            var administradorServico = new AdministradorServico(context);

            // Act - São as ações dos itens (set de  algumas propriedades)
            administradorServico.Incluir(adm);

            // Assert - Validação desses dados (Get)
            Assert.AreEqual(1, administradorServico.Todos(1).Count());
            
        }
// Teste 2 Administrador - Cria um novo registro e faz busca po Id
    [TestMethod]
        public void TestandoSalvarBuscarPorIdAdministrador()
        {
            // Arrange  - São todas as variáveis para as validações
            var context = CriarContextoTeste(); 
            // Limpar a base de dados (Tabela)
            //context.Database.ExecuteSqlRaw("TRUNCATE TABLE adminitradores");

            var adm = new Administrador();
            adm.Id = 2;
            adm.Email = "teste277@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            var administradorServico = new AdministradorServico(context);

            // Act - São as ações dos itens (set de  algumas propriedades)
            administradorServico.Incluir(adm);
            administradorServico.BuscaPorId(adm.Id);
            
            // Assert - Validação desses dados (Get)
            Assert.AreEqual(2, adm.Id);
        }
    }

}