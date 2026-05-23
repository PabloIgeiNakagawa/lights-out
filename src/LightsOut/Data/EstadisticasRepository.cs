namespace LightsOut.Data;

public static class EstadisticasRepository
{
    private const string RecordFile = "lightout_data.txt";
    private static readonly int[] record = new int[9];
    private static readonly int[] jugadas = new int[9];
    private static readonly int[] ganadas = new int[9];
    private static readonly int[] tiempoTotal = new int[9];
    private static readonly int[] racha = new int[9];
    private static readonly int[] mejorRacha = new int[9];

    static EstadisticasRepository() => CargarDatos();

    public static void RegistrarVictoria(int n, int turnos, int tiempoPartida)
    {
        jugadas[n]++;
        ganadas[n]++;
        racha[n]++;
        if (racha[n] > mejorRacha[n]) mejorRacha[n] = racha[n];
        if (record[n] == 0 || turnos < record[n]) record[n] = turnos;
        tiempoTotal[n] += tiempoPartida;
        GuardarDatos();
    }

    public static void CargarDatos()
    {
        try
        {
            foreach (var linea in File.ReadLines(RecordFile))
            {
                var parts = linea.Split(',');
                if (parts.Length < 7) continue;
                int n = int.Parse(parts[0]);
                record[n] = int.Parse(parts[1]);
                jugadas[n] = int.Parse(parts[2]);
                ganadas[n] = int.Parse(parts[3]);
                tiempoTotal[n] = int.Parse(parts[4]);
                racha[n] = int.Parse(parts[5]);
                mejorRacha[n] = int.Parse(parts[6]);
            }
        }
        catch
        {
            for (int n = 3; n <= 8; n++)
            {
                record[n] = 0;
                jugadas[n] = 0;
                ganadas[n] = 0;
                tiempoTotal[n] = 0;
                racha[n] = 0;
                mejorRacha[n] = 0;
            }
        }
    }

    public static void GuardarDatos()
    {
        using var sw = new StreamWriter(RecordFile);
        for (int n = 3; n <= 8; n++)
        {
            sw.WriteLine($"{n},{record[n]},{jugadas[n]},{ganadas[n]},{tiempoTotal[n]},{racha[n]},{mejorRacha[n]}");
        }
    }

    public static void ResetStats()
    {
        for (int n = 3; n <= 8; n++)
        {
            record[n] = 0;
            jugadas[n] = 0;
            ganadas[n] = 0;
            tiempoTotal[n] = 0;
            racha[n] = 0;
            mejorRacha[n] = 0;
        }
        GuardarDatos();
    }

    public static int GetRecord(int n) => record[n];
    public static int GetJugadas(int n) => jugadas[n];
    public static int GetGanadas(int n) => ganadas[n];
    public static int GetTiempoTotal(int n) => tiempoTotal[n];
    public static int GetRacha(int n) => racha[n];
    public static int GetMejorRacha(int n) => mejorRacha[n];

    public static string GetTiempoPromedio(int n)
    {
        if (ganadas[n] == 0) return "--";
        int segs = tiempoTotal[n] / ganadas[n];
        return $"{segs / 60:D2}:{segs % 60:D2}";
    }
}
