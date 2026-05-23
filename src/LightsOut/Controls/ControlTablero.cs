using System.Drawing.Drawing2D;

namespace LightsOut.Controls;

// Grilla N×N de botones con iconos circulares pintados en Bitmap.
// Maneja el feedback visual al presionar una celda, los bordes de pista
// y los 4 esquemas de color intercambiables en caliente.
public class ControlTablero : UserControl
{
    // Cada esquema define dos colores: [apagado (claro), encendido (oscuro)]
    private static readonly Color[][] Esquemas =
    [
        [Color.FromArgb(50, 200, 50),  Color.FromArgb(30, 30, 30)],    // Clásico
        [Color.FromArgb(60, 130, 255), Color.FromArgb(20, 20, 50)],    // Noche
        [Color.FromArgb(255, 200, 50), Color.FromArgb(180, 40, 40)],   // Fuego
        [Color.FromArgb(100, 220, 220), Color.FromArgb(0, 60, 100)],   // Hielo
    ];

    public static readonly string[] NombresEsquemas = ["Clásico", "Noche", "Fuego", "Hielo"];

    // Color amarillo semitransparente para el feedback de fila/columna
    private static readonly Color ColorResalte = Color.FromArgb(180, 255, 255, 100);

    private readonly Button[,] botones;
    private readonly int tamano;
    private int esquemaActual;
    private int tamanoIcono;
    private Bitmap iconoApagado;
    private Bitmap iconoEncendido;
    private System.Windows.Forms.Timer timerFeedback;

    public int TamanoGrid => tamano;
    public int EsquemaActual => esquemaActual;

    // Construye un TableLayoutPanel con N×N botones FlatStyle.
    // Cada botón tiene Tag = (fila, columna) para identificar la celda.
    public ControlTablero(int tamano, EventHandler clickHandler)
    {
        this.tamano = tamano;
        this.tamanoIcono = Math.Max(40, Math.Min(70, 400 / tamano));
        GenerarIconos();

        var grilla = new TableLayoutPanel
        {
            RowCount = tamano,
            ColumnCount = tamano,
            Dock = DockStyle.None,
            AutoSize = true,
            BackColor = Color.FromArgb(240, 240, 240),
        };

        grilla.ColumnStyles.Clear();
        grilla.RowStyles.Clear();
        for (int i = 0; i < tamano; i++)
        {
            grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, tamanoIcono + 16));
            grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, tamanoIcono + 16));
        }

        botones = new Button[tamano, tamano];
        var btnSize = new Size(tamanoIcono + 16, tamanoIcono + 16);

        for (int f = 0; f < tamano; f++)
        {
            for (int c = 0; c < tamano; c++)
            {
                var btn = new Button
                {
                    Size = btnSize,
                    FlatStyle = FlatStyle.Flat,
                    Tag = (f, c),
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += clickHandler;
                botones[f, c] = btn;
                grilla.Controls.Add(btn, c, f);
            }
        }

        Controls.Add(grilla);
        grilla.Left = (Width - grilla.Width) / 2;
        grilla.Top = (Height - grilla.Height) / 2;

        ActualizarTodos(new bool[tamano, tamano]);
    }

    // Pinta dos Bitmap con círculos: uno claro (apagado) y uno oscuro (encendido),
    // usando los colores del esquema actual.
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
            // Brillo especular en la parte superior izquierda
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

    // Cambia el icono de un botón individual según el estado de la luz.
    public void ActualizarBoton(int fila, int columna, bool encendida)
    {
        var img = encendida ? iconoEncendido : iconoApagado;
        botones[fila, columna].Image = img != null ? new Bitmap(img) : null;
        botones[fila, columna].BackColor = Color.Transparent;
    }

    // Refresca todos los botones desde una matriz de estado.
    public void ActualizarTodos(bool[,] estado)
    {
        for (int f = 0; f < tamano; f++)
            for (int c = 0; c < tamano; c++)
                ActualizarBoton(f, c, estado[f, c]);
    }

    // Resalta temporalmente (250ms) toda la fila y columna de la celda presionada.
    public void MostrarFeedback(int fila, int columna)
    {
        timerFeedback?.Stop();

        for (int c = 0; c < tamano; c++)
            botones[fila, c].BackColor = ColorResalte;
        for (int f = 0; f < tamano; f++)
            botones[f, columna].BackColor = ColorResalte;

        timerFeedback = new System.Windows.Forms.Timer { Interval = 250 };
        timerFeedback.Tick += (_, _) =>
        {
            for (int c = 0; c < tamano; c++)
                botones[fila, c].BackColor = Color.Transparent;
            for (int f = 0; f < tamano; f++)
                botones[f, columna].BackColor = Color.Transparent;
            timerFeedback.Stop();
        };
        timerFeedback.Start();
    }

    // Pone un borde de 3px del color indicado sobre un botón (para la pista).
    public void MarcarBoton(int fila, int columna, Color colorBorde)
    {
        var fa = botones[fila, columna].FlatAppearance;
        fa.BorderColor = colorBorde;
        fa.BorderSize = 3;
        botones[fila, columna].Invalidate();
    }

    // Restaura el borde a 0px (sin borde).
    public void LimpiarBorde(int fila, int columna)
    {
        var fa = botones[fila, columna].FlatAppearance;
        fa.BorderSize = 0;
        botones[fila, columna].Invalidate();
    }

    // Cambia el esquema de color por índice. Regenera los iconos.
    public bool CambiarEsquemaColor(int indice)
    {
        if (indice < 0 || indice >= Esquemas.Length) return false;
        esquemaActual = indice;
        GenerarIconos();
        return true;
    }
}

// Extensiones auxiliares para oscurecer o aclarar un Color.
internal static class ColorExtensions
{
    public static Color Darker(this Color c) => Color.FromArgb(c.A, Math.Max(0, c.R - 40), Math.Max(0, c.G - 40), Math.Max(0, c.B - 40));
    public static Color Brighter(this Color c) => Color.FromArgb(c.A, Math.Min(255, c.R + 40), Math.Min(255, c.G + 40), Math.Min(255, c.B + 40));
}
