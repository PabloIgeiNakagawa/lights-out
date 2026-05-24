using LightsOut.Controls;
using LightsOut.Data;
using LightsOut.Model;
using LightsOut.Sound;

namespace LightsOut.Forms;

// Orquestador de la partida: conecta el modelo (Tablero), la vista (ControlTablero),
// el sonido (GeneradorSonido) y la persistencia (EstadisticasRepository).
// Maneja los eventos de click sobre la grilla, el timer de partida,
// el sistema de pistas y el mute.
public class PantallaJuego : UserControl
{
    public event EventHandler VolverSolicitado;

    public Size TamanioRecomendado
    {
        get
        {
            int n = tamano;
            int iconoSize = Math.Max(40, Math.Min(70, 400 / n));
            int btnSize = iconoSize + 16;
            int gridPx = n * btnSize;
            int ancho = Math.Max(620, gridPx + 60);
            int alto = Math.Max(500, gridPx + 130);
            return new Size(ancho, alto);
        }
    }

    private readonly int tamano;

    public string textoVentana => $"Lights Out - {tamano}x{tamano}";

    private readonly ControlTablero vistaTablero;

    private readonly Label contadorDeTurnos;
    private readonly Label recordDeTurnos;
    private readonly Label labelTiempo;
    private readonly Button botonPista;
    private readonly Button botonMute;
    private readonly TrackBar sliderVolumen;
    private readonly ComboBox comboColor;

    private int volumenAnterior = 50;

    public PantallaJuego(int tamano)
    {
        this.tamano = tamano;
        Dock = DockStyle.Fill;

        // --- Norte: info (Volver, Turnos, Tiempo, Record) ---
        var panelInfo = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(5),
        };

        var botonVolver = new BotonJuego("Volver");
        botonVolver.Click += (_, _) => AccionVolver();
        panelInfo.Controls.Add(botonVolver);

        contadorDeTurnos = new Label { Text = "Turnos: 0", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true };
        panelInfo.Controls.Add(contadorDeTurnos);

        labelTiempo = new Label { Text = "Tiempo: 00:00", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true };
        panelInfo.Controls.Add(labelTiempo);

        recordDeTurnos = new Label
        {
            Text = $"Record: {EstadisticasRepository.GetRecord(tamano)}",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
        };
        panelInfo.Controls.Add(recordDeTurnos);

        // --- Centro: grilla del tablero ---
        vistaTablero = new ControlTablero(new Tablero(tamano));
        vistaTablero.Dock = DockStyle.None;
        vistaTablero.AutoSize = true;
        vistaTablero.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        vistaTablero.Anchor = AnchorStyles.None;
        vistaTablero.CeldaPresionada += OnCeldaPresionada;
        vistaTablero.Victoria += OnVictoria;
        vistaTablero.TickSegundo += segs => labelTiempo.Text = $"Tiempo: {FormatearTiempo(segs)}";

        // --- Sur: acciones (Reiniciar, Pista, Silenciar, Volumen, Color) ---
        var panelInferior = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, // Llena su celda asignada en la cuadrícula
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(5),
        };

        var botonReiniciar = new BotonJuego("Reiniciar");
        botonReiniciar.Click += (_, _) => AccionReiniciar();
        panelInferior.Controls.Add(botonReiniciar);

        botonPista = new BotonJuego("Pista");
        botonPista.Click += (_, _) => vistaTablero.DarPista();
        panelInferior.Controls.Add(botonPista);

        botonMute = new BotonJuego("Silenciar");
        botonMute.Size = new Size(130, 32);
        botonMute.AutoSize = false;
        botonMute.Click += (_, _) => ToggleMute();
        panelInferior.Controls.Add(botonMute);

        sliderVolumen = new TrackBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 50,
            Width = 90,
            TickFrequency = 10,
        };
        // El slider refleja el volumen en GeneradorSonido en tiempo real (0.0 a 1.0)
        sliderVolumen.ValueChanged += (_, _) =>
        {
            GeneradorSonido.Volumen = sliderVolumen.Value / 100f;
            botonMute.Text = sliderVolumen.Value == 0 ? "Activar sonido" : "Silenciar";
        };
        panelInferior.Controls.Add(sliderVolumen);

        panelInferior.Controls.Add(new Label { Text = "Color:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });

        comboColor = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 120,
        };
        comboColor.Items.AddRange(ControlTablero.NombresEsquemas);
        comboColor.SelectedIndex = 0;
        // Al cambiar el esquema, se regeneran los iconos y se refresca toda la grilla
        comboColor.SelectedIndexChanged += (_, _) =>
        {
            vistaTablero.CambiarEsquemaColor(comboColor.SelectedIndex);
        };
        panelInferior.Controls.Add(comboColor);

        // --- Creación del Contenedor Principal (Cuadrícula invisible) ---
        var contenedorPrincipal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(20) // Margen de cortesía alrededor de todo el juego
        };

        // Definimos las proporciones de las 3 filas
        contenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // Fila 0: Info superior
        contenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 1: Tablero (toma todo el resto)
        contenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // Fila 2: Controles inferiores

        // Asignamos cada panel y el tablero a su respectiva fila
        contenedorPrincipal.Controls.Add(panelInfo, 0, 0);
        contenedorPrincipal.Controls.Add(vistaTablero, 0, 1);
        contenedorPrincipal.Controls.Add(panelInferior, 0, 2);

        // Agregamos únicamente la cuadrícula estructurada al UserControl
        Controls.Add(contenedorPrincipal);
    }

    private static string FormatearTiempo(int segs) => $"{segs / 60:D2}:{segs % 60:D2}";

    // Vuelve al menú. Si la partida tiene turnos, pide confirmación.
    private void AccionVolver()
    {
        if (vistaTablero.Tablero.Turnos > 0)
        {
            var r = MessageBox.Show(
                "Hay una partida en curso. Si vuelve al menú se contará como partida perdida.\n¿Desea continuar?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;

            EstadisticasRepository.RegistrarAbandono(tamano, vistaTablero.Tablero.TiempoPartida);
        }
        vistaTablero.DetenerTimer();
        VolverSolicitado?.Invoke(this, EventArgs.Empty);
    }

    private void AccionReiniciar()
    {
        vistaTablero.Reiniciar();
        labelTiempo.Text = "Tiempo: 00:00";
        contadorDeTurnos.Text = "Turnos: 0";
    }

    private void OnCeldaPresionada(int fila, int columna, int turnos)
    {
        contadorDeTurnos.Text = $"Turnos: {turnos}";
    }

    private void OnVictoria(int turnos, int tiempo)
    {
        EstadisticasRepository.RegistrarVictoria(tamano, turnos, tiempo);
        recordDeTurnos.Text = $"Record: {EstadisticasRepository.GetRecord(tamano)}";

        MessageBox.Show(
            $"Felicitaciones, Ganaste!\n\n" +
            $"Tablero: {tamano}x{tamano}\nTurnos: {turnos}\nTiempo: {FormatearTiempo(tiempo)}\n" +
            $"Partidas jugadas: {EstadisticasRepository.GetJugadas(tamano)}\n" +
            $"Partidas ganadas: {EstadisticasRepository.GetGanadas(tamano)}\n" +
            $"Record histórico: {EstadisticasRepository.GetRecord(tamano)} movimientos",
            "Victoria!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        AccionReiniciar();
    }

    // Alterna entre silencio y el volumen anterior.
    private void ToggleMute()
    {
        if (sliderVolumen.Value > 0)
        {
            volumenAnterior = sliderVolumen.Value;
            sliderVolumen.Value = 0;
        }
        else
        {
            sliderVolumen.Value = volumenAnterior > 0 ? volumenAnterior : 50;
        }
    }
}
