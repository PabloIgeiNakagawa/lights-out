// Form principal de la aplicación.
// No usa MDI ni diseñador gráfico. Contiene un Panel contenedor donde se
// intercambian las pantallas (menú, juego, estadísticas) eliminando y agregando
// UserControls según la navegación.

namespace LightsOut.Forms;

public class VentanaPrincipal : Form
{
    private readonly Panel panelPrincipal;
    private PantallaJuego panelJuego;
    private PantallaMenu panelMenu;

    // Configura la ventana sin posibilidad de redimensionar y arranca con el menú.
    public VentanaPrincipal()
    {
        Text = "Lights Out";
        Size = new Size(500, 550);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        panelPrincipal = new Panel { Dock = DockStyle.Fill };
        Controls.Add(panelPrincipal);

        MostrarMenu();
    }

    // Vuelve al menú principal. Descarta PantallaJuego si existe y ajusta el tamaño base.
    public void MostrarMenu()
    {
        panelJuego?.Dispose();
        panelJuego = null;

        panelMenu?.Dispose();
        panelMenu = new PantallaMenu(this);

        panelPrincipal.Controls.Clear();
        panelPrincipal.Controls.Add(panelMenu);
        panelMenu.Dock = DockStyle.Fill;

        Size = new Size(500, 550);
        CenterToScreen();
        Text = "Lights Out";
    }

    // Inicia una partida del tamaño dado. Calcula el tamaño dinámico de la ventana
    // según la cantidad de celdas para que la grilla entre holgada.
    public void MostrarJuego(int tamano)
    {
        panelJuego?.Dispose();
        panelJuego = new PantallaJuego(this, tamano);

        panelPrincipal.Controls.Clear();
        panelPrincipal.Controls.Add(panelJuego);
        panelJuego.Dock = DockStyle.Fill;

        Text = $"Lights Out - {tamano}x{tamano}";

        int iconoSize = Math.Max(40, Math.Min(70, 400 / tamano));
        int btnSize = iconoSize + 16;
        int gridPx = tamano * btnSize;
        int ancho = Math.Max(620, gridPx + 60);
        int alto = Math.Max(500, gridPx + 130);
        Size = new Size(ancho, alto);
        CenterToScreen();
    }

    // Muestra la pantalla de estadísticas con la tabla resumen.
    public void MostrarEstadisticas()
    {
        panelJuego?.Dispose();
        panelJuego = null;

        var panel = new PantallaEstadisticas(this);
        panelPrincipal.Controls.Clear();
        panelPrincipal.Controls.Add(panel);
        panel.Dock = DockStyle.Fill;

        Size = new Size(680, 480);
        CenterToScreen();
        Text = "Lights Out - Estadísticas";
    }
}
