using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculoTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange  - São todas as variáveis para as validações
            var veiculo = new Veiculo();

            // Act - São as ações dos itens (set de  algumas propriedades)
            veiculo.Id = 1;
            veiculo.Nome = "Ford Ranger";
            veiculo.Marca = "Ford";
            veiculo.Ano = 2025;

            // Assert - Validação desses dados (Get)
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("Ford Ranger", veiculo.Nome);
            Assert.AreEqual("Ford", veiculo.Marca);
            Assert.AreEqual(2025, veiculo.Ano);

        }
    }
}