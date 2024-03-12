using Bogus;
using JornadaMilhas.Dominio.ValueObjects;

namespace JornadaMilhas.Integration.Test.API.DataBuilders;

internal class PeriodoDataBuilder : Faker<Periodo>
{
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public PeriodoDataBuilder()
    {
        CustomInstantiator(f =>
        {
            DateTime dataInicio = DataInicial ?? f.Date.Soon();
            DateTime dataFinal = DataFinal ?? dataInicio.AddDays(30);
            return new Periodo(dataInicio, dataFinal);
        });
    }

    public Periodo Build() => Generate();
}
