using JornadaMilhas.Dominio.Entidades;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API
{
    public class Rota_GET : IClassFixture<JornadaMilhasWebApplicationFactory>
    {
        private readonly JornadaMilhasWebApplicationFactory app;
        public Rota_GET(JornadaMilhasWebApplicationFactory app)
        {
            this.app = app;
        }

        [Fact]
        public async Task Recupera_Rota_PorId()
        {
            //Arrange
            var rotaExistente = app.Context.Rota.FirstOrDefault();
            if (rotaExistente is null)
            {
                rotaExistente = new Rota()
                {
                    Origem = "Origem",
                    Destino = "Destino"
                };
                app.Context.Add(rotaExistente);
                app.Context.SaveChanges();
            }

            using var client = await app.GetClientWithAccessTokenAsync();

            //Act
            var response = await client.GetFromJsonAsync<Rota>("/rota-viagem/" + rotaExistente.Id);

            //Assert
            Assert.NotNull(response);  
            Assert.Equal(rotaExistente.Origem, response.Origem);
            Assert.Equal(rotaExistente.Destino, response.Destino);

        }
    }
}
