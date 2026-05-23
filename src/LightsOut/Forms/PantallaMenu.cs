using LightsOut.Controls;
using LightsOut.Data;

namespace LightsOut.Forms;

// Menú principal. Fondo animado (GIF embebido) con overlay semitransparente,
// botones de dificultad (Fácil 4x4, Intermedio 5x5, Difícil 6x6),
// spinner para tamaño personalizado (3–8) y acceso a estadísticas.
public class PantallaMenu : UserControl
{
    private readonly VentanaPrincipal ventanaJuego;
    private readonly NumericUpDown spinnerTamano;
    private readonly Label labelStats;
    private static readonly string[] Niveles = ["Fácil", "Intermedio", "Difícil"];
    private static readonly int[] Tamanos = [4, 5, 6];

    // Construye toda la UI del menú: fondo, overlay, títulos, botones, spinner y stats.
    public PantallaMenu(VentanaPrincipal ventanaJuego)
    {
        this.ventanaJuego = ventanaJuego;
        Dock = DockStyle.Fill;

        // Fondo oscuro sólido.
        var overlay = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(30, 30, 40),
        };

        // TableLayoutPanel con los controles del menú, centrado manualmente
        var central = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(30),
            ColumnCount = 1,
        };
        central.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        overlay.Resize += (_, _) =>
        {
            central.Left = (overlay.Width - central.Width) / 2;
            central.Top = (overlay.Height - central.Height) / 2;
        };
        overlay.Controls.Add(central);

        // Título
        var titulo = new Label
        {
            Text = "LIGHT OUT",
            Font = new Font("Segoe UI", 42, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
        };
        central.Controls.Add(titulo);

        // Subtítulo
        central.Controls.Add(new Label
        {
            Text = "Apaga todas las luces!",
            Font = new Font("Segoe UI", 16),
            ForeColor = Color.FromArgb(200, 200, 200),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 15),
        });

        // Separador visual
        central.Controls.Add(new Label
        {
            Text = "━━━━━━━━━━━━━━━━━━━━",
            ForeColor = Color.Gray,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10),
        });

        // Botones de dificultad predefinidos
        for (int i = 0; i < Niveles.Length; i++)
        {
            var btn = new BotonMenu(Niveles[i]) { Margin = new Padding(0, 6, 0, 6) };
            int idx = i;
            btn.Click += (_, _) => ventanaJuego.MostrarJuego(Tamanos[idx]);
            central.Controls.Add(btn);
        }

        // Separador
        central.Controls.Add(new Label
        {
            Text = "━━━━━━━━━━━━━━━━━━━━",
            ForeColor = Color.Gray,
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 10),
        });

        // Fila: label "Tamaño personalizado:" + NumericUpDown
        var panelSpinner = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };
        panelSpinner.Controls.Add(new Label
        {
            Text = "Tamaño personalizado:",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14),
            AutoSize = true,
            Margin = new Padding(0, 5, 5, 0),
        });

        spinnerTamano = new NumericUpDown
        {
            Minimum = 3,
            Maximum = 8,
            Value = 4,
            Width = 70,
            TextAlign = HorizontalAlignment.Center,
        };
        panelSpinner.Controls.Add(spinnerTamano);
        central.Controls.Add(panelSpinner);

        // Botón jugar con tamaño personalizado
        var btnJugar = new BotonMenu("Jugar") { Margin = new Padding(0, 6, 0, 6) };
        btnJugar.Click += (_, _) => ventanaJuego.MostrarJuego((int)spinnerTamano.Value);
        central.Controls.Add(btnJugar);

        // Botón estadísticas
        var btnStats = new BotonMenu("Estadísticas") { Margin = new Padding(0, 6, 0, 6) };
        btnStats.Click += (_, _) => ventanaJuego.MostrarEstadisticas();
        central.Controls.Add(btnStats);

        // Label de resumen de estadísticas
        labelStats = new Label
        {
            ForeColor = Color.FromArgb(180, 255, 180),
            Font = new Font("Segoe UI", 12),
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 0),
        };
        central.Controls.Add(labelStats);

        Load += (_, _) => CentrarControles(central, titulo);

        Controls.Add(overlay);

        ActualizarEstadisticas();
    }

    // Actualiza el texto de estadísticas globales (total partidas, ganadas, mejor récord).
    public void ActualizarEstadisticas()
    {
        int totalJug = 0, totalGan = 0, mejorRecord = 0;
        for (int n = 3; n <= 8; n++)
        {
            totalJug += EstadisticasRepository.GetJugadas(n);
            totalGan += EstadisticasRepository.GetGanadas(n);
            int r = EstadisticasRepository.GetRecord(n);
            if (r > 0 && (mejorRecord == 0 || r < mejorRecord))
                mejorRecord = r;
        }
        labelStats.Text = totalJug > 0
            ? $"Récord: {mejorRecord} mov  |  Partidas: {totalJug}  |  Ganadas: {totalGan}"
            : "Bienvenido! Aún no hay partidas guardadas.";
    }

    private static void CentrarControles(TableLayoutPanel p, Control excepto)
    {
        int m = p.Controls.Cast<Control>().Where(c => c != excepto).Max(c => c.Width);
        foreach (Control c in p.Controls)
            if (c != excepto)
                c.Margin = new Padding((m - c.Width) / 2, c.Margin.Top, 0, c.Margin.Bottom);
    }
}
