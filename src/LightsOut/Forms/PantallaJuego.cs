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
            int n = tablero.Tamano;
            int iconoSize = Math.Max(40, Math.Min(70, 400 / n));
            int btnSize = iconoSize + 16;
            int gridPx = n * btnSize;
            int ancho = Math.Max(620, gridPx + 60);
            int alto = Math.Max(500, gridPx + 130);
            return new Size(ancho, alto);
        }
    }

    public string textoVentana => $"Lights Out - {tablero.Tamano}x{tablero.Tamano}";

    private readonly Tablero tablero;
    private readonly ControlTablero vistaTablero;

    private readonly Label contadorDeTurnos;
    private readonly Label recordDeTurnos;
    private readonly Label labelTiempo;
    private readonly Button botonPista;
    private readonly Button botonMute;
    private readonly TrackBar sliderVolumen;
    private readonly ComboBox comboColor;

    private readonly System.Windows.Forms.Timer timerSegundo;
    private int segundosTranscurridos;
    private int volumenAnterior = 50;

    // Lista de movimientos óptimos calculados por el solver para la función "Pista"
    private List<int[]> movimientosPista = new();
    private int indicePista;

    // Inicializa modelo, construye UI y arranca el timer de 1 segundo.
    public PantallaJuego(int tamano)
    {
        tablero = new Tablero(tamano);
        tablero.Randomizar();

        Dock = DockStyle.Fill;

        // --- Norte: info (Volver, Turnos, Tiempo, Record) ---
        var panelInfo = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
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
        vistaTablero = new ControlTablero(tamano, BtnClickHandler);
        vistaTablero.Dock = DockStyle.Fill;

        // --- Sur: acciones (Reiniciar, Pista, Silenciar, Volumen, Color) ---
        var panelInferior = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(5),
        };

        var botonReiniciar = new BotonJuego("Reiniciar");
        botonReiniciar.Click += (_, _) => AccionReiniciar();
        panelInferior.Controls.Add(botonReiniciar);

        botonPista = new BotonJuego("Pista");
        botonPista.Click += (_, _) => AccionPista();
        panelInferior.Controls.Add(botonPista);

        botonMute = new BotonJuego("Silenciar");
        botonMute.Size = new Size(130, 28);
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
            vistaTablero.ActualizarTodos(EstadoMatriz());
        };
        panelInferior.Controls.Add(comboColor);

        // --- Timer que cuenta los segundos de partida ---
        timerSegundo = new System.Windows.Forms.Timer { Interval = 1000 };
        timerSegundo.Tick += (_, _) =>
        {
            segundosTranscurridos++;
            labelTiempo.Text = $"Tiempo: {FormatearTiempo(segundosTranscurridos)}";
        };
        timerSegundo.Start();

        Controls.Add(panelInfo);
        Controls.Add(vistaTablero);
        Controls.Add(panelInferior);
    }

    // Convierte el estado del modelo a matriz bool[,] para actualizar la vista.
    private bool[,] EstadoMatriz()
    {
        int n = tablero.Tamano;
        var estado = new bool[n, n];
        for (int f = 0; f < n; f++)
            for (int c = 0; c < n; c++)
                estado[f, c] = tablero.EstaEncendidaLuz(f, c);
        return estado;
    }

    private static string FormatearTiempo(int segs) => $"{segs / 60:D2}:{segs % 60:D2}";

    // Vuelve al menú. Si la partida tiene turnos, pide confirmación.
    private void AccionVolver()
    {
        if (tablero.Turnos > 0)
        {
            var r = MessageBox.Show(
                "Hay una partida en curso. Volver al menú perderá el progreso.\n¿Desea continuar?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;
        }
        timerSegundo.Stop();
        VolverSolicitado?.Invoke(this, EventArgs.Empty);
    }

    // Genera un nuevo tablero aleatorio y resetea contadores, timer y pistas.
    private void AccionReiniciar()
    {
        tablero.Randomizar();
        segundosTranscurridos = 0;
        labelTiempo.Text = "Tiempo: 00:00";
        contadorDeTurnos.Text = "Turnos: 0";
        movimientosPista.Clear();
        indicePista = 0;
        vistaTablero.ActualizarTodos(EstadoMatriz());
    }

    // Revela un movimiento óptimo por vez usando el solver del Tablero.
    // Si la lista se agotó, recalcula la solución completa.
    private void AccionPista()
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
            vistaTablero.MarcarBoton(f, c, Color.Red);

            // Timer de 1.5s para limpiar el borde rojo automáticamente
            var limpiar = new System.Windows.Forms.Timer { Interval = 1500 };
            limpiar.Tick += (_, _) =>
            {
                vistaTablero.LimpiarBorde(f, c);
                limpiar.Stop();
            };
            limpiar.Start();

            indicePista++;
            GeneradorSonido.Pista();
        }
    }

    // Handler principal de clicks sobre celdas de la grilla.
    // Lee la coordenada desde Tag, aplica la jugada, actualiza la UI,
    // sincroniza la lista de pistas y chequea si el jugador ganó.
    private void BtnClickHandler(object sender, EventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not (int fila, int columna)) return;

        tablero.Presionar(fila, columna);
        vistaTablero.MostrarFeedback(fila, columna);
        GeneradorSonido.Click();

        // Actualiza solo la fila y columna afectadas (el toggle cambió esas celdas)
        int n = tablero.Tamano;
        for (int c = 0; c < n; c++)
            vistaTablero.ActualizarBoton(fila, c, tablero.EstaEncendidaLuz(fila, c));
        for (int f = 0; f < n; f++)
            vistaTablero.ActualizarBoton(f, columna, tablero.EstaEncendidaLuz(f, columna));

        // Si el jugador tocó una celda que estaba en la solución de pista,
        // la saca de la lista. Si tocó otra, invalida la pista actual.
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

        contadorDeTurnos.Text = $"Turnos: {tablero.Turnos}";

        int turnosFinales = tablero.Turnos;
        tablero.TiempoPartida = segundosTranscurridos;

        if (tablero.EsTerminado())
        {
            EstadisticasRepository.RegistrarVictoria(n, turnosFinales, segundosTranscurridos);
            timerSegundo.Stop();
            GeneradorSonido.Victoria();
            recordDeTurnos.Text = $"Record: {EstadisticasRepository.GetRecord(n)}";

            MessageBox.Show(
                $"Felicitaciones, Ganaste!\n\n" +
                $"Tablero: {n}x{n}\nTurnos: {turnosFinales}\nTiempo: {FormatearTiempo(segundosTranscurridos)}\n" +
                $"Partidas jugadas: {EstadisticasRepository.GetJugadas(n)}\n" +
                $"Partidas ganadas: {EstadisticasRepository.GetGanadas(n)}\n" +
                $"Record histórico: {EstadisticasRepository.GetRecord(n)} movimientos",
                "Victoria!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            AccionReiniciar();
            timerSegundo.Start();
        }
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
