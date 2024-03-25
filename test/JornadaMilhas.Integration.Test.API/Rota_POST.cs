using JornadaMilhas.Dominio.Entidades;
using System.Net;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API
{
    public class Rota_POST:IClassFixture<JornadaMilhasWebApplicationFactory>
    {
        private readonly JornadaMilhasWebApplicationFactory app;

        public Rota_POST(JornadaMilhasWebApplicationFactory app)
        {
            this.app = app;
        }

        [Fact]
        public async Task Cadastra_Rota()
        {
            //Arrange
           using var client = await app.GetClientWithAccessTokenAsync();

            var rotaViagem = new Rota()
            {
              Origem = "Origem",
              Destino = "Destino"
            };
            //Act
            var response = await client.PostAsJsonAsync("/rota-viagem", rotaViagem);

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Cadastra_Rota_SemAutorizacao()
        {
            //Arrange   

            using var client = app.CreateClient();

            var rotaViagem = new Rota()
            {
                Origem = "Origem",
                Destino = "Destino"
            };
            //Act
            var response = await client.PostAsJsonAsync("/rota-viagem", rotaViagem);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
