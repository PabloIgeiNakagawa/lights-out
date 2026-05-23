using Xunit;
using LightsOut.Model;

namespace LightsOut.Tests;

public class TableroTest
{
    [Fact]
    public void ConstructorCreaTableroDelTamanoCorrecto()
    {
        for (int n = 3; n <= 8; n++)
        {
            var t = new Tablero(n);
            Assert.Equal(n, t.Tamano);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    Assert.False(t.EstaEncendidaLuz(i, j));
        }
    }

    [Fact]
    public void RandomNoLanzaExcepcion()
    {
        var t = new Tablero(5);
        t.Randomizar();
    }

    [Fact]
    public void PresionarToggleaFilaYColumna()
    {
        var t = new Tablero(4);
        t.Presionar(0, 0);
        for (int j = 0; j < 4; j++) Assert.True(t.EstaEncendidaLuz(0, j));
        for (int i = 1; i < 4; i++) Assert.True(t.EstaEncendidaLuz(i, 0));
        Assert.False(t.EstaEncendidaLuz(1, 1));
    }

    [Fact]
    public void PresionarIncrementaTurnos()
    {
        var t = new Tablero(4);
        Assert.Equal(0, t.Turnos);
        t.Presionar(0, 0);
        Assert.Equal(1, t.Turnos);
        t.Presionar(1, 1);
        Assert.Equal(2, t.Turnos);
        t.Presionar(2, 2);
        Assert.Equal(3, t.Turnos);
    }

    [Fact]
    public void PresionarDosVecesVuelveAlEstadoAnterior()
    {
        var t = new Tablero(4);
        Assert.False(t.EstaEncendidaLuz(2, 2));
        t.Presionar(2, 2);
        Assert.True(t.EstaEncendidaLuz(2, 2));
        t.Presionar(2, 2);
        Assert.False(t.EstaEncendidaLuz(2, 2));
    }

    [Fact]
    public void EsTerminadoFalseSiHayAlgunaEncendida()
    {
        var t = new Tablero(4);
        t.Presionar(0, 0);
        Assert.False(t.EsTerminado());
    }

    [Fact]
    public void EsTerminadoTrueSiTodasApagadas()
    {
        var t = new Tablero(4);
        Assert.True(t.EsTerminado());
    }

    [Fact]
    public void ResolverParaParSiempreEncuentraSolucion()
    {
        foreach (int n in new[] { 4, 6, 8 })
        {
            for (int trial = 0; trial < 20; trial++)
            {
                var t = new Tablero(n);
                t.Randomizar();
                var sol = t.Resolver();
                Assert.NotNull(sol);
            }
        }
    }

    [Fact]
    public void ResolverSolucionEsValida()
    {
        for (int n = 3; n <= 8; n++)
        {
            for (int trial = 0; trial < 20; trial++)
            {
                var t = new Tablero(n);
                t.Randomizar();
                var sol = t.Resolver();
                Assert.NotNull(sol);

                foreach (var m in sol)
                    t.Presionar(m[0], m[1]);

                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        Assert.False(t.EstaEncendidaLuz(i, j));
            }
        }
    }

    [Fact]
    public void ResolverNoModificaContadorDeTurnos()
    {
        var t = new Tablero(4);
        t.Randomizar();
        int turnosAntes = t.Turnos;
        t.Resolver();
        Assert.Equal(turnosAntes, t.Turnos);
    }

    [Fact]
    public void ResolverImparSiempreEncuentraSolucion()
    {
        foreach (int n in new[] { 3, 5, 7 })
        {
            for (int trial = 0; trial < 20; trial++)
            {
                var t = new Tablero(n);
                t.Randomizar();
                var sol = t.Resolver();
                Assert.NotNull(sol);
            }
        }
    }
}
