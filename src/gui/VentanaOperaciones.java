package gui;

import java.awt.Color;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;

import operaciones.procesosLightOut;

public class VentanaOperaciones extends JFrame implements ActionListener 
{

	private JButton[][] botonesDeJuego;							
	procesosLightOut procesosLightOut;
	private JLabel contadorDeTurnos;
	private JLabel recordDeTurnos;
	private JButton botonVolver;
	
	
	public VentanaOperaciones(int tamano) 
	{
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setTitle("Lights Out");
		setSize(500, 500);
		
		procesosLightOut = new procesosLightOut(tamano);
		
		JPanel panelPrincipal = new JPanel();
		JPanel panelDeBotones = new JPanel();
		
		panelDeBotones.setBounds(0, 14, 484, 433);
		botonesDeJuego = new JButton[tamano][tamano];				
		panelDeBotones.setLayout(new GridLayout(tamano,tamano));
		
		procesosLightOut.random();
		
		for(int fila = 0; fila < tamano; fila++)						
		{
			for(int columna = 0; columna < tamano; columna++)
			{
				JButton boton = new JButton();			
				botonesDeJuego[fila][columna] = boton;				
				boton.setName(""+fila+columna);	
				encenderApagarLuzBoton(fila, columna); 
				boton.addActionListener(this);			
				panelDeBotones.add(boton);				
			}
		}
		panelPrincipal.setLayout(null);
		panelPrincipal.add(panelDeBotones);		
		setContentPane(panelPrincipal);	
		
		botonVolver = new JButton("Volver");
		botonVolver.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				VentanaPrincipal ventana = new VentanaPrincipal();
				ventana.setVisible(true);
				dispose();
			}
		});
		botonVolver.setName("volver");
		botonVolver.setBounds(395, 443, 89, 23);
		
		contadorDeTurnos = new JLabel("Contador de Turnos: " + procesosLightOut.turnosRealizados());
		contadorDeTurnos.setBounds(0, 0, 484, 14);
		
		recordDeTurnos = new JLabel("Record: " + procesosLightOut.recordTurnos() );
		recordDeTurnos.setBounds(0, 447, 484, 14);
		
		panelPrincipal.add(botonVolver);
		panelPrincipal.add(contadorDeTurnos);
		panelPrincipal.add(recordDeTurnos);
		
	}		
	
	@Override
	public void actionPerformed(ActionEvent e) 
	{
		JButton boton = (JButton)e.getSource();		
		String ubicacion = boton.getName();	
		
		if(ubicacion.equals("volver")) 
		{
			VentanaPrincipal ventanaPrincipal = new VentanaPrincipal();
			ventanaPrincipal.setVisible(true);
			dispose();
		}
		
		char filaUbicacion = ubicacion.charAt(0);				
		char columnaUbicacion = ubicacion.charAt(1);				
		int fila = Character.getNumericValue(filaUbicacion);	
		int columna = Character.getNumericValue(columnaUbicacion);
		
		procesosLightOut.presionar(fila, columna);
		
		encenderApagarBotonesCorrectos(fila, columna);
		
		contadorDeTurnos.setText("Contador de Turnos: " + procesosLightOut.turnosRealizados());
		
		if(procesosLightOut.juegoTerminado()) 
		{
			JOptionPane.showMessageDialog(this, "Felicitaciones, Ganaste el Juego");
			recordDeTurnos.setText("Record: " + procesosLightOut.recordTurnos());
			reiniciarJuego();
			contadorDeTurnos.setText("Contador de Turnos: " + procesosLightOut.turnosRealizados());
				
		}
									
	}
	
	private void encenderApagarBotonesCorrectos(int fila, int columna) 
	{
		for(int columnasVecinas = 0; columnasVecinas < botonesDeJuego.length; columnasVecinas++)
		{
			encenderApagarLuzBoton(fila, columnasVecinas);
		}
		for(int filasVecinas = 0; filasVecinas < botonesDeJuego.length; filasVecinas++)
		{
			encenderApagarLuzBoton(filasVecinas, columna);
		}	
	}
	
	private void encenderApagarLuzBoton(int fila, int columna) 
	{
		if(procesosLightOut.estaEncendidaLuz(fila, columna) == false) 
		{
			botonesDeJuego[fila][columna].setBackground(Color.GREEN);
		}
		else 
		{
			botonesDeJuego[fila][columna].setBackground(Color.BLACK);
		}
	}
	
	private void reiniciarJuego() 
	{
		procesosLightOut.random();
		for(int fila = 0; fila < botonesDeJuego.length; fila++)						
		{
			for(int columna = 0; columna < botonesDeJuego[0].length; columna++)
			{
				encenderApagarLuzBoton(fila, columna);
			}
		}
	}
	
}
