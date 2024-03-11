using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JornadaMilhas.Integration.Test.API
{
    public class OfertaViagem_DELETE : IClassFixture<JornadaMilhasWebApplicationFactory>
    {
        private readonly JornadaMilhasWebApplicationFactory app;

        public OfertaViagem_DELETE(JornadaMilhasWebApplicationFactory app)
        {
            this.app = app;
        }

        [Fact]
        public async Task Deletar_OfertaViagem_PorId() {
            //Arrange
            var ofertaExistente = app.Context.OfertasViagem.FirstOrDefault();
            if (ofertaExistente is null)
            {
                ofertaExistente = new OfertaViagem()
                {
                    Preco = 100,
                    Rota = new Rota("Origem", "Destino"),
                    Periodo = new Periodo(DateTime.Parse("2024-03-03"), DateTime.Parse("2024-03-06"))
                };
                app.Context.Add(ofertaExistente);
                app.Context.SaveChanges();
            }

            using var client = await app.GetClientWithAccessTokenAsync();

            //Act
            var response = await client.DeleteAsync("/ofertas-viagem/" + ofertaExistente.Id);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        }


    }
}
