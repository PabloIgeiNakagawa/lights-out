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
        panelMenu = new PantallaMenu();
        panelMenu.JuegoSolicitado += (_, t) => MostrarJuego(t);
        panelMenu.EstadisticasSolicitadas += (_, _) => MostrarEstadisticas();

        panelPrincipal.Controls.Clear();
        panelPrincipal.Controls.Add(panelMenu);
        panelMenu.Dock = DockStyle.Fill;

        ClientSize = panelMenu.TamanioRecomendado;
        CenterToScreen();
    }

    // Inicia una partida del tamaño dado. Calcula el tamaño dinámico de la ventana
    // según la cantidad de celdas para que la grilla entre holgada.
    public void MostrarJuego(int tamano)
    {
        panelJuego?.Dispose();
        panelJuego = new PantallaJuego(tamano);
        panelJuego.VolverSolicitado += (_, _) => MostrarMenu();

        panelPrincipal.Controls.Clear();
        panelPrincipal.Controls.Add(panelJuego);
        panelJuego.Dock = DockStyle.Fill;

        Text = panelJuego.textoVentana;
        ClientSize = panelJuego.TamanioRecomendado;
        CenterToScreen();
    }

    // Muestra la pantalla de estadísticas con la tabla resumen.
    public void MostrarEstadisticas()
    {
        panelJuego?.Dispose();
        panelJuego = null;

        var panel = new PantallaEstadisticas();
        panel.VolverSolicitado += (_, _) => MostrarMenu();

        panelPrincipal.Controls.Clear();
        panelPrincipal.Controls.Add(panel);
        panel.Dock = DockStyle.Fill;

        Text = panel.textoVentana;
        ClientSize = panel.TamanioRecomendado;
        CenterToScreen();
    }
}
