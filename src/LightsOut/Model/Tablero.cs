namespace LightsOut.Model;

public class Tablero
{
    private readonly bool[,] tabla;
    private int turnos;
    private int tiempoPartida;

    public Tablero(int tamano)
    {
        tabla = new bool[tamano, tamano];
    }

    public int TiempoPartida
    {
        get => tiempoPartida;
        set => tiempoPartida = value;
    }

    public int Turnos => turnos;
    public int Tamano => tabla.GetLength(0);

    public void Randomizar()
    {
        int n = tabla.GetLength(0);
        for (int f = 0; f < n; f++)
            for (int c = 0; c < n; c++)
                tabla[f, c] = false;

        for (int f = 0; f < n; f++)
            for (int c = 0; c < n; c++)
                if (Random.Shared.NextDouble() > 0.5)
                    ToggleFilaColumna(f, c);

        turnos = 0;
    }

    private void ToggleFilaColumna(int fila, int columna)
    {
        int n = tabla.GetLength(0);
        for (int c = 0; c < n; c++)
            tabla[fila, c] = !tabla[fila, c];
        for (int f = 0; f < n; f++)
            if (f != fila)
                tabla[f, columna] = !tabla[f, columna];
    }

    public void Presionar(int fila, int columna)
    {
        turnos++;
        ToggleFilaColumna(fila, columna);
    }

    public bool EsTerminado()
    {
        int n = tabla.GetLength(0);
        for (int f = 0; f < n; f++)
            for (int c = 0; c < n; c++)
                if (tabla[f, c])
                    return false;
        return true;
    }

    public bool EstaEncendidaLuz(int fila, int columna) => tabla[fila, columna];

    public int[][] Resolver()
    {
        int n = tabla.GetLength(0);
        if (n % 2 == 0)
            return ResolverPar();
        else
            return ResolverImpar();
    }

    private int[][] ResolverPar()
    {
        int n = tabla.GetLength(0);
        var B = new bool[n];
        var Bcol = new bool[n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (tabla[i, j])
                {
                    B[i] = !B[i];
                    Bcol[j] = !Bcol[j];
                }
        return EmpaquetarSolucion((i, j) => B[i] ^ Bcol[j] ^ tabla[i, j], n);
    }

    private int[][] ResolverImpar()
    {
        int n = tabla.GetLength(0);
        var B = new bool[n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (tabla[i, j]) B[i] = !B[i];

        bool P = B[0];
        for (int i = 1; i < n; i++)
            if (B[i] != P) return null!;

        for (int j = 0; j < n; j++)
        {
            bool colPar = false;
            for (int i = 0; i < n; i++)
                if (tabla[i, j]) colPar = !colPar;
            if (colPar != P) return null!;
        }

        return EmpaquetarSolucion((i, j) =>
        {
            bool r = (i == 0) ? P : false;
            bool c = (j == 0) ? P : false;
            return r ^ c ^ tabla[i, j];
        }, n);
    }

    private static int[][] EmpaquetarSolucion(Func<int, int, bool> cond, int n)
    {
        int count = 0;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (cond(i, j)) count++;

        int idx = 0;
        var r = new int[count][];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (cond(i, j))
                    r[idx++] = new[] { i, j };
        return r;
    }
}
