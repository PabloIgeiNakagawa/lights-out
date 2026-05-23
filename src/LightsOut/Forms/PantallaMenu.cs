using LightsOut.Controls;
using LightsOut.Data;

namespace LightsOut.Forms;

// Menú principal. Fondo animado (GIF embebido) con overlay semitransparente,
// botones de dificultad (Fácil 4x4, Intermedio 5x5, Difícil 6x6),
// spinner para tamaño personalizado (3–8) y acceso a estadísticas.
public class PantallaMenu : UserControl
{
    public Size TamanioRecomendado => new(500, 550);

    public event EventHandler<int> JuegoSolicitado;
    public event EventHandler EstadisticasSolicitadas;

    private readonly NumericUpDown spinnerTamano;
    private static readonly string[] Niveles = ["Fácil", "Intermedio", "Difícil"];
    private static readonly int[] Tamanos = [4, 6, 8];

    // Construye toda la UI del menú: fondo, overlay, títulos, botones, spinner y stats.
    public PantallaMenu()
    {
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
            Text = "LIGHTS OUT",
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
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 0, 0, 15),
        });

        // Separador visual
        central.Controls.Add(new Label
        {
            Text = "━━━━━━━━━━━━━━━━━━━━",
            ForeColor = Color.Gray,
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 0, 0, 10),
        });

        // Botones de dificultad predefinidos
        for (int i = 0; i < Niveles.Length; i++)
        {
            var btn = new BotonMenu(Niveles[i]) { Margin = new Padding(0, 6, 0, 6), Anchor = AnchorStyles.None };
            int idx = i;
            btn.Click += (_, _) => JuegoSolicitado?.Invoke(this, Tamanos[idx]);
            central.Controls.Add(btn);
        }

        // Separador
        central.Controls.Add(new Label
        {
            Text = "━━━━━━━━━━━━━━━━━━━━",
            ForeColor = Color.Gray,
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Margin = new Padding(0, 10, 0, 10),
        });

        // Fila: label "Tamaño personalizado:" + NumericUpDown
        var panelSpinner = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Anchor = AnchorStyles.None,
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
        var btnJugar = new BotonMenu("Jugar") { Margin = new Padding(0, 6, 0, 6), Anchor = AnchorStyles.None };
        btnJugar.Click += (_, _) => JuegoSolicitado?.Invoke(this, (int)spinnerTamano.Value);
        central.Controls.Add(btnJugar);

        // Botón estadísticas
        var btnStats = new BotonMenu("Estadísticas") { Margin = new Padding(0, 6, 0, 6), Anchor = AnchorStyles.None };
        btnStats.Click += (_, _) => EstadisticasSolicitadas?.Invoke(this, EventArgs.Empty);
        central.Controls.Add(btnStats);

        Controls.Add(overlay);
    }
}
