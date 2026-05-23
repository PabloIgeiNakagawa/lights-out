using System.Data;
using LightsOut.Controls;
using LightsOut.Data;

namespace LightsOut.Forms;

// Pantalla que muestra una tabla resumen de estadísticas para todos los tamaños (3×3 a 8×8).
// Usa DataTable como fuente del DataGridView para ordenamiento y formato automáticos.
public class PantallaEstadisticas : UserControl
{
    public event EventHandler VolverSolicitado;

    public Size TamanioRecomendado => new(850, 480);

    public string textoVentana => "Lights Out - Estadísticas";

    private readonly DataGridView tabla;
    private readonly DataTable modeloTabla;

    public PantallaEstadisticas()
    {
        Dock = DockStyle.Fill;
        Padding = new Padding(30);

        var contenedorPrincipal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
        };

        // Fila 1: Título (alto fijo de 60)
        contenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        // Fila 2: Tabla (ocupa todo el espacio restante disponible)
        contenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        // Fila 3: Botones (alto fijo de 60)
        contenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

        // Título centrado arriba
        var titulo = new Label
        {
            Text = "ESTADÍSTICAS",
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
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
            ColumnHeadersHeight = 58
        };

        CargarDatos();

        // Botones inferiores (Volver, Resetear estadísticas)
        var flowBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
        };

        var btnVolver = new BotonMenu("Volver");
        btnVolver.Click += (_, _) => VolverSolicitado?.Invoke(this, EventArgs.Empty);
        flowBotones.Controls.Add(btnVolver);

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

        // Agregamos los controles a sus respectivas celdas de la cuadrícula
        contenedorPrincipal.Controls.Add(titulo, 0, 0);       // Fila 0
        contenedorPrincipal.Controls.Add(tabla, 0, 1);        // Fila 1
        contenedorPrincipal.Controls.Add(panelBotones, 0, 2); // Fila 2

        // Finalmente, agregamos la cuadrícula al UserControl
        Controls.Add(contenedorPrincipal);
    }

    // Refresca las filas de la tabla desde EstadisticasRepository.
    private void CargarDatos()
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
