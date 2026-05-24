using System.Text.Json;

namespace LightsOut.Data;

public static class EstadisticasRepository
{
    private const string RecordFile = "lightout_data.json";
    private static readonly Dictionary<int, Estadistica> datos = [];

    static EstadisticasRepository() => CargarDatos();

    public static void RegistrarVictoria(int n, int turnos, int tiempoPartida)
    {
        var d = Obtener(n);
        d.Jugadas++;
        d.Ganadas++;
        d.Racha++;
        if (d.Racha > d.MejorRacha) d.MejorRacha = d.Racha;
        if (d.Record == 0 || turnos < d.Record) d.Record = turnos;
        d.TiempoTotal += tiempoPartida;
        GuardarDatos();
    }

    public static void CargarDatos()
    {
        try
        {
            var json = File.ReadAllText(RecordFile);
            var lista = JsonSerializer.Deserialize<List<Estadistica>>(json);
            if (lista != null)
            {
                datos.Clear();
                foreach (var e in lista)
                    datos[e.Tamano] = e;
                return;
            }
        }
        catch { }

        datos.Clear();
        for (int n = 3; n <= 8; n++)
            datos[n] = new Estadistica { Tamano = n };
    }

    public static void GuardarDatos()
    {
        var lista = new List<Estadistica>(datos.Values);
        var json = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(RecordFile, json);
    }

    public static void ResetStats()
    {
        datos.Clear();
        for (int n = 3; n <= 8; n++)
            datos[n] = new Estadistica { Tamano = n };
        GuardarDatos();
    }

    private static Estadistica Obtener(int n) =>
        datos.TryGetValue(n, out var d) ? d : (datos[n] = new Estadistica { Tamano = n });

    public static int GetRecord(int n) => Obtener(n).Record;
    public static int GetJugadas(int n) => Obtener(n).Jugadas;
    public static int GetGanadas(int n) => Obtener(n).Ganadas;
    public static int GetTiempoTotal(int n) => Obtener(n).TiempoTotal;
    public static int GetRacha(int n) => Obtener(n).Racha;
    public static int GetMejorRacha(int n) => Obtener(n).MejorRacha;

    public static string GetTiempoPromedio(int n)
    {
        var d = Obtener(n);
        if (d.Ganadas == 0) return "--";
        int segs = d.TiempoTotal / d.Ganadas;
        return $"{segs / 60:D2}:{segs % 60:D2}";
    }
}
