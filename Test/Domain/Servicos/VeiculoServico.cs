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
    public class VeiculoServicoTest
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
// Teste 1 Veiculo - Cria um registro na tabela Veiculos para teste
        [TestMethod]
        public void TestandoSalvarVeiculo()
        {
            // Arrange  - São todas as variáveis para as validações
            var context = CriarContextoTeste(); 
            // Limpar a base de dados (Tabela)
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");

            var veiculo = new Veiculo();
            veiculo.Id = 1;
            veiculo.Nome = "Ford Ranger";
            veiculo.Marca = "Ford";
            veiculo.Ano = 2025;

            var veiculoServico = new VeiculoServico(context);

            // Act - São as ações dos itens (set de  algumas propriedades)
            veiculoServico.Incluir(veiculo);

            // Assert - Validação desses dados (Get)
            Assert.AreEqual(1, veiculoServico.Todos(1).Count());
            
        }
        
// Teste 2 Veiculo - Adciona mais um Veiculo e seleciona por Id
    [TestMethod]
        public void TestandoSalvarVeiculoBuscarPorId()
        {
            // Arrange  - São todas as variáveis para as validações
            var context = CriarContextoTeste(); 
            // Limpar a base de dados (Tabela)
            //context.Database.ExecuteSqlRaw("TRUNCATE TABLE adminitradores");

            var veiculo = new Veiculo();
            veiculo.Id = 2;
            veiculo.Nome = "Gol 1.0";
            veiculo.Marca = "Wolkswargem";
            veiculo.Ano = 2010;

            var veiculoServico = new VeiculoServico(context);

            // Act - São as ações dos itens (set de  algumas propriedades)
            veiculoServico.Incluir(veiculo);
            veiculoServico.BuscaPorId(veiculo.Id);
            
            // Assert - Validação desses dados (Get)
            Assert.AreEqual(2, veiculo.Id);
        }
// Teste 3 Veiculo - Salva um novo Veiculo e altera os dados do Veiculo(Id=2)

        [TestMethod]
        public void TestandoSalvarAtualizarVeiculo()
        {
            // Arrange  - São todas as variáveis para as validações
            var context = CriarContextoTeste(); 
            // Limpar a base de dados (Tabela)
            //context.Database.ExecuteSqlRaw("TRUNCATE TABLE adminitradores");

            var veiculo = new Veiculo();
            veiculo.Id = 3;
            veiculo.Nome = "Gol 1.0";
            veiculo.Marca = "Wolkswargem";
            veiculo.Ano = 2010;

            var veiculoServico = new VeiculoServico(context);

            // Act - São as ações dos itens (set de  algumas propriedades)
            veiculoServico.Incluir(veiculo);
            veiculo = veiculoServico.BuscaPorId(2);
            if(veiculo!=null)
            {
                veiculo.Id = 2;
                veiculo.Nome = "X3";
                veiculo.Marca = "BMW";
                veiculo.Ano = 2020;
                veiculoServico.Atualizar(veiculo);
            }

            // Assert - Validação desses dados (Get)
            Assert.AreEqual(3, veiculoServico.Todos(1).Count()); 
        }

// Teste 4 Veiculo - Testando Apagar um Veiculo

        [TestMethod]
        
        public void TestandoApagarVeiculo()
        {
            // Arrange  - São todas as variáveis para as validações
            var context = CriarContextoTeste(); 

            var veiculoServico = new VeiculoServico(context);

            // Act - São as ações dos itens (set de  algumas propriedades)
            var veiculo = new Veiculo();
            veiculo = veiculoServico.BuscaPorId(1);
            if(veiculo!=null)
                veiculoServico.Apagar(veiculo);

            // Assert - Validação desses dados (Get)
            Assert.AreEqual(2, veiculoServico.Todos(1).Count()); 
        }
    }
}