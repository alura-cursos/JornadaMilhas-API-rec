using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace JornadaMilhas.Integration.Test.API
{
    public class OfertaViagem_POST
    {
        [Fact]
        public async Task Cadastra_OfertaViagem()
        {
            //Arrange
            var app = new JornadaMilhasWebApplicationFactory();

            using var client = app.CreateClient();

            var ofertaViagem = new OfertaViagem()
            {
                Preco = 100,
                Rota = new Rota("Origem", "Destino"),
                Periodo = new Periodo(DateTime.Parse("2024-03-03"), DateTime.Parse("2024-03-06"))
            };
            //Act
            var response = await client.PostAsJsonAsync("/ofertas-viagem", ofertaViagem);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
