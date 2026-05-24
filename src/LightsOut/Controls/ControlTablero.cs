using LightsOut.Model;
using LightsOut.Sound;
using System.Drawing.Drawing2D;

namespace LightsOut.Controls;

public class ControlTablero : UserControl
{
    private static readonly Color[][] Esquemas =
    [
        [Color.FromArgb(50, 200, 50),  Color.FromArgb(30, 30, 30)],
        [Color.FromArgb(60, 130, 255), Color.FromArgb(20, 20, 50)],
        [Color.FromArgb(255, 200, 50), Color.FromArgb(180, 40, 40)],
        [Color.FromArgb(100, 220, 220), Color.FromArgb(0, 60, 100)],
    ];

    public static readonly string[] NombresEsquemas = ["Clásico", "Noche", "Fuego", "Hielo"];

    private static readonly Color ColorResalte = Color.FromArgb(180, 255, 255, 100);

    private readonly Button[,] botones;
    private readonly Tablero tablero;
    private readonly TableLayoutPanel grilla;
    private int esquemaActual;
    private int tamanoIcono;
    private Bitmap iconoApagado;
    private Bitmap iconoEncendido;
    private System.Windows.Forms.Timer timerFeedback;
    private readonly System.Windows.Forms.Timer timerSegundo = new() { Interval = 1000 };

    private List<int[]> movimientosPista = new();
    private int indicePista;

    public event Action<int, int, int> CeldaPresionada;
    public event Action<int, int> Victoria;
    public event Action<int> TickSegundo;

    public int TamanoGrid => tablero.Tamano;
    public int EsquemaActual => esquemaActual;
    public Tablero Tablero => tablero;

    public ControlTablero(Tablero tablero)
    {
        this.tablero = tablero;
        tablero.Randomizar();
        this.tamanoIcono = Math.Max(40, Math.Min(70, 400 / tablero.Tamano));
        GenerarIconos();

        int n = tablero.Tamano;
        grilla = new TableLayoutPanel
        {
            RowCount = n,
            ColumnCount = n,
            Dock = DockStyle.None,
            AutoSize = true,
            BackColor = Color.FromArgb(240, 240, 240),
        };

        grilla.ColumnStyles.Clear();
        grilla.RowStyles.Clear();
        for (int i = 0; i < n; i++)
        {
            grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, tamanoIcono + 16));
            grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, tamanoIcono + 16));
        }

        botones = new Button[n, n];
        var btnSize = new Size(tamanoIcono + 16, tamanoIcono + 16);

        for (int f = 0; f < n; f++)
        {
            for (int c = 0; c < n; c++)
            {
                var btn = new Button
                {
                    Size = btnSize,
                    FlatStyle = FlatStyle.Flat,
                    Tag = (f, c),
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += BtnClickHandler;
                botones[f, c] = btn;
                grilla.Controls.Add(btn, c, f);
            }
        }

        Controls.Add(grilla);

        Resize += (_, _) =>
        {
            grilla.Left = (Width - grilla.Width) / 2;
            grilla.Top = (Height - grilla.Height) / 2;
        };

        timerSegundo.Tick += (_, _) =>
        {
            tablero.TiempoPartida++;
            TickSegundo?.Invoke(tablero.TiempoPartida);
        };
        timerSegundo.Start();

        ActualizarTodos();
    }

    public void DetenerTimer() => timerSegundo.Stop();

    public void Reiniciar()
    {
        tablero.Randomizar();
        movimientosPista.Clear();
        indicePista = 0;
        tablero.TiempoPartida = 0;
        ActualizarTodos();
        timerSegundo.Start();
    }

    public void DarPista()
    {
        if (movimientosPista.Count == 0 || indicePista >= movimientosPista.Count)
        {
            var sol = tablero.Resolver();
            if (sol == null || sol.Length == 0)
            {
                MessageBox.Show("No se encontró solución. Puede que ya esté resuelto.",
                    "Pista", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GeneradorSonido.Error();
                return;
            }
            movimientosPista = [.. sol];
            indicePista = 0;
        }

        if (indicePista < movimientosPista.Count)
        {
            var m = movimientosPista[indicePista];
            int f = m[0], c = m[1];
            MarcarBoton(f, c, Color.Red);

            var limpiar = new System.Windows.Forms.Timer { Interval = 1500 };
            limpiar.Tick += (_, _) =>
            {
                LimpiarBorde(f, c);
                limpiar.Stop();
            };
            limpiar.Start();

            indicePista++;
            GeneradorSonido.Pista();
        }
    }

    private void BtnClickHandler(object sender, EventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not (int fila, int columna)) return;

        tablero.Presionar(fila, columna);
        MostrarFeedback(fila, columna);
        GeneradorSonido.Click();

        int n = tablero.Tamano;
        for (int c = 0; c < n; c++)
            ActualizarBoton(fila, c);
        for (int f = 0; f < n; f++)
            ActualizarBoton(f, columna);

        bool encontrado = false;
        for (int i = 0; i < movimientosPista.Count; i++)
        {
            var m = movimientosPista[i];
            if (m[0] == fila && m[1] == columna)
            {
                movimientosPista.RemoveAt(i);
                if (i < indicePista) indicePista--;
                encontrado = true;
                break;
            }
        }
        if (!encontrado)
        {
            movimientosPista.Clear();
            indicePista = 0;
        }

        CeldaPresionada?.Invoke(fila, columna, tablero.Turnos);

        if (tablero.EsTerminado())
        {
            timerSegundo.Stop();
            GeneradorSonido.Victoria();
            Victoria?.Invoke(tablero.Turnos, tablero.TiempoPartida);
        }
    }

    private void GenerarIconos()
    {
        iconoApagado?.Dispose();
        iconoEncendido?.Dispose();

        Color colorOff = Esquemas[esquemaActual][0];
        Color colorOn = Esquemas[esquemaActual][1];

        iconoApagado = new Bitmap(tamanoIcono, tamanoIcono);
        using (var g = Graphics.FromImage(iconoApagado))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(colorOff);
            g.FillEllipse(brush, 3, 3, tamanoIcono - 6, tamanoIcono - 6);
            using var pen = new Pen(colorOff.Darker(), 1);
            g.DrawEllipse(pen, 3, 3, tamanoIcono - 6, tamanoIcono - 6);
            using var highlight = new SolidBrush(Color.FromArgb(64, Color.White));
            g.FillEllipse(highlight, tamanoIcono / 3, tamanoIcono / 3, tamanoIcono / 3, tamanoIcono / 3);
        }

        iconoEncendido = new Bitmap(tamanoIcono, tamanoIcono);
        using (var g = Graphics.FromImage(iconoEncendido))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(colorOn);
            g.FillEllipse(brush, 3, 3, tamanoIcono - 6, tamanoIcono - 6);
            using var pen = new Pen(colorOn.Brighter(), 1);
            g.DrawEllipse(pen, 3, 3, tamanoIcono - 6, tamanoIcono - 6);
        }
    }

    public void ActualizarBoton(int fila, int columna)
    {
        bool encendida = tablero.EstaEncendidaLuz(fila, columna);
        var img = encendida ? iconoEncendido : iconoApagado;
        botones[fila, columna].Image = img != null ? new Bitmap(img) : null;
        botones[fila, columna].BackColor = Color.Transparent;
    }

    public void ActualizarTodos()
    {
        int n = tablero.Tamano;
        for (int f = 0; f < n; f++)
            for (int c = 0; c < n; c++)
                ActualizarBoton(f, c);
    }

    public void MostrarFeedback(int fila, int columna)
    {
        timerFeedback?.Stop();

        for (int c = 0; c < tablero.Tamano; c++)
            botones[fila, c].BackColor = ColorResalte;
        for (int f = 0; f < tablero.Tamano; f++)
            botones[f, columna].BackColor = ColorResalte;

        timerFeedback = new System.Windows.Forms.Timer { Interval = 250 };
        timerFeedback.Tick += (_, _) =>
        {
            for (int c = 0; c < tablero.Tamano; c++)
                botones[fila, c].BackColor = Color.Transparent;
            for (int f = 0; f < tablero.Tamano; f++)
                botones[f, columna].BackColor = Color.Transparent;
            timerFeedback.Stop();
        };
        timerFeedback.Start();
    }

    public void MarcarBoton(int fila, int columna, Color colorBorde)
    {
        var fa = botones[fila, columna].FlatAppearance;
        fa.BorderColor = colorBorde;
        fa.BorderSize = 3;
        botones[fila, columna].Invalidate();
    }

    public void LimpiarBorde(int fila, int columna)
    {
        var fa = botones[fila, columna].FlatAppearance;
        fa.BorderSize = 0;
        botones[fila, columna].Invalidate();
    }

    public bool CambiarEsquemaColor(int indice)
    {
        if (indice < 0 || indice >= Esquemas.Length) return false;
        esquemaActual = indice;
        GenerarIconos();
        ActualizarTodos();
        return true;
    }
}

internal static class ColorExtensions
{
    public static Color Darker(this Color c) => Color.FromArgb(c.A, Math.Max(0, c.R - 40), Math.Max(0, c.G - 40), Math.Max(0, c.B - 40));
    public static Color Brighter(this Color c) => Color.FromArgb(c.A, Math.Min(255, c.R + 40), Math.Min(255, c.G + 40), Math.Min(255, c.B + 40));
}
