using System.Data;
using LightsOut.Controls;
using LightsOut.Data;

namespace LightsOut.Forms;

// Pantalla que muestra una tabla resumen de estadísticas para todos los tamaños (3×3 a 8×8).
// Usa DataTable como fuente del DataGridView para ordenamiento y formato automáticos.
public class PantallaEstadisticas : UserControl
{
    private readonly VentanaPrincipal ventanaJuego;
    private readonly DataGridView tabla;
    private readonly DataTable modeloTabla;

    public PantallaEstadisticas(VentanaPrincipal ventanaJuego)
    {
        this.ventanaJuego = ventanaJuego;
        Dock = DockStyle.Fill;
        Padding = new Padding(30);

        // Título centrado arriba
        var titulo = new Label
        {
            Text = "ESTADÍSTICAS",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 60,
        };

        // Tabla con 7 columnas, solo lectura
        modeloTabla = new DataTable();
        modeloTabla.Columns.Add("Tamaño", typeof(string));
        modeloTabla.Columns.Add("Récord", typeof(string));
        modeloTabla.Columns.Add("Jugadas", typeof(int));
        modeloTabla.Columns.Add("Ganadas", typeof(int));
        modeloTabla.Columns.Add("T.Promedio", typeof(string));
        modeloTabla.Columns.Add("Racha", typeof(int));
        modeloTabla.Columns.Add("Mejor racha", typeof(int));

        tabla = new DataGridView
        {
            DataSource = modeloTabla,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Font = new Font("Segoe UI", 14),
            RowHeadersVisible = false,
            Dock = DockStyle.Fill,
        };

        CargarDatos();

        Controls.Add(titulo);
        Controls.Add(tabla);

        // Botones inferiores (Cerrar, Resetear estadísticas)
        var flowBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
        };

        var btnCerrar = new BotonMenu("Cerrar");
        btnCerrar.Click += (_, _) => ventanaJuego.MostrarMenu();
        flowBotones.Controls.Add(btnCerrar);

        var btnReset = new BotonMenu("Resetear estadísticas") { Margin = new Padding(15, 6, 0, 6) };
        btnReset.Click += (_, _) =>
        {
            var r = MessageBox.Show(
                "¿Resetear todas las estadísticas?\nEsta acción no se puede deshacer.",
                "Confirmar reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                EstadisticasRepository.ResetStats();
                CargarDatos();
            }
        };
        flowBotones.Controls.Add(btnReset);

        // Panel contenedor para centrar los botones horizontalmente
        var panelBotones = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
        };
        flowBotones.Location = new Point((panelBotones.Width - flowBotones.Width) / 2, 10);
        panelBotones.Resize += (_, _) =>
        {
            flowBotones.Location = new Point((panelBotones.Width - flowBotones.Width) / 2, 10);
        };
        panelBotones.Controls.Add(flowBotones);
        Controls.Add(panelBotones);
    }

    // Refresca las filas de la tabla desde EstadisticasRepository.
    public void CargarDatos()
    {
        modeloTabla.Rows.Clear();
        for (int n = 3; n <= 8; n++)
        {
            string tam = $"{n}x{n}";
            int rec = EstadisticasRepository.GetRecord(n);
            string record = rec == 0 ? "--" : rec.ToString();
            int jug = EstadisticasRepository.GetJugadas(n);
            int gan = EstadisticasRepository.GetGanadas(n);
            string tProm = EstadisticasRepository.GetTiempoPromedio(n);
            int racha = EstadisticasRepository.GetRacha(n);
            int mRacha = EstadisticasRepository.GetMejorRacha(n);
            modeloTabla.Rows.Add(tam, record, jug, gan, tProm, racha, mRacha);
        }
    }
}
