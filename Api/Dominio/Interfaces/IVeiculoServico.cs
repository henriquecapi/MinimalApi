using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);

        void Incluir(Veiculo veiculo);

        Veiculo? BuscaPorId(int id);

        void Atualizar(Veiculo veiculo);

        void Apagar(Veiculo veiculo);

    }
}