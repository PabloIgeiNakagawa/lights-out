using Xunit;
using LightsOut.Data;

namespace LightsOut.Tests;

public class EstadisticasRepositoryTest : IDisposable
{
    private const string DataFile = "lightout_data.json";
    private readonly string backup;

    public EstadisticasRepositoryTest()
    {
        if (File.Exists(DataFile))
        {
            backup = DataFile + ".bak";
            File.Copy(DataFile, backup, true);
        }
    }

    public void Dispose()
    {
        File.Delete(DataFile);
        if (backup != null && File.Exists(backup))
            File.Move(backup, DataFile, true);
    }

    private void CleanDataFile()
    {
        File.Delete(DataFile);
        EstadisticasRepository.ResetStats();
    }

    [Fact]
    public void PersistenciaGuardaYCargaRecord()
    {
        CleanDataFile();
        EstadisticasRepository.RegistrarVictoria(4, 2, 0);
        Assert.Equal(2, EstadisticasRepository.GetRecord(4));
        Assert.Equal(1, EstadisticasRepository.GetJugadas(4));
        Assert.Equal(1, EstadisticasRepository.GetGanadas(4));
    }

    [Fact]
    public void PersistenciaRecordSeActualizaConMejor()
    {
        CleanDataFile();
        EstadisticasRepository.RegistrarVictoria(4, 5, 0);
        Assert.Equal(5, EstadisticasRepository.GetRecord(4));

        EstadisticasRepository.RegistrarVictoria(4, 3, 0);
        Assert.Equal(3, EstadisticasRepository.GetRecord(4));
    }

    [Fact]
    public void PersistenciaStatsSonPorSeparadoPorTamanio()
    {
        CleanDataFile();
        EstadisticasRepository.RegistrarVictoria(3, 6, 0);
        Assert.Equal(0, EstadisticasRepository.GetRecord(4));
        Assert.Equal(0, EstadisticasRepository.GetJugadas(4));
    }

    [Fact]
    public void ResetStatsLimpiaTodo()
    {
        CleanDataFile();
        EstadisticasRepository.RegistrarVictoria(4, 2, 0);
        Assert.Equal(2, EstadisticasRepository.GetRecord(4));

        EstadisticasRepository.ResetStats();

        Assert.Equal(0, EstadisticasRepository.GetRecord(4));
        Assert.Equal(0, EstadisticasRepository.GetJugadas(4));
        Assert.Equal(0, EstadisticasRepository.GetGanadas(4));
    }

    [Fact]
    public void AbandonoIncrementaJugadasPeroNoGanadas()
    {
        CleanDataFile();
        EstadisticasRepository.RegistrarAbandono(4, 30);
        Assert.Equal(1, EstadisticasRepository.GetJugadas(4));
        Assert.Equal(0, EstadisticasRepository.GetGanadas(4));
        Assert.Equal(0, EstadisticasRepository.GetRecord(4));
    }

    [Fact]
    public void AbandonoReseteaRacha()
    {
        CleanDataFile();
        EstadisticasRepository.RegistrarVictoria(4, 2, 0);
        Assert.Equal(1, EstadisticasRepository.GetRacha(4));

        EstadisticasRepository.RegistrarAbandono(4, 10);
        Assert.Equal(2, EstadisticasRepository.GetJugadas(4));
        Assert.Equal(1, EstadisticasRepository.GetGanadas(4));
        Assert.Equal(0, EstadisticasRepository.GetRacha(4));
    }
}
