package gui;

import javax.swing.JFrame;
import javax.swing.JButton;
import javax.swing.JLabel;
import java.awt.event.ActionListener;
import java.awt.event.ActionEvent;
import javax.swing.ImageIcon;
import java.awt.Color;
import java.awt.Font;

public class VentanaPrincipal extends JFrame implements ActionListener {
	
	private JButton[] botonesNiveles ;
	private int espacioEntreBotones = 200;
	private String  niveles [] = {"Facil", "Intermedio", "Dificil"}; 

	public VentanaPrincipal() {
		
		JLabel lblNewLabel_1 = new JLabel("LIGHT OUT");
		lblNewLabel_1.setForeground(new Color(255, 255, 255));
		lblNewLabel_1.setFont(new Font("Tahoma", Font.PLAIN, 38));
		lblNewLabel_1.setBackground(new Color(255, 255, 255));
		lblNewLabel_1.setBounds(149, 40, 206, 44);
		getContentPane().add(lblNewLabel_1);
		
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setSize(500,500);
		getContentPane().setLayout(null);
		
		botonesNiveles = new JButton [niveles.length];
		for(int f = 0; f < 3; f++) 
		{
			JButton boton = new JButton(niveles[f]);			
			botonesNiveles[f] = boton;				
			boton.setName(""+niveles[f]);
			boton.setBounds(200, espacioEntreBotones, 110, 30);
			boton.addActionListener(this);			
			getContentPane().add(boton);
			espacioEntreBotones+=70;	
		}
		
		JLabel lblNewLabel = new JLabel("");
		lblNewLabel.setBounds(0, 0, 484, 461);
		lblNewLabel.setIcon(new ImageIcon(getClass().getResource("/gui/imagenes/luces.gif")));
		getContentPane().add(lblNewLabel);	
		
}

	@Override
	public void actionPerformed(ActionEvent e) 
	{
		JButton boton = (JButton)e.getSource();		
		String ubicacion = boton.getName();	
		
		if(ubicacion.equals("Facil")) 
		{
			VentanaOperaciones miVentana = new VentanaOperaciones(4);
			miVentana.setVisible(true);
			dispose();
		}
		if(ubicacion.equals("Intermedio")) 
		{
			VentanaOperaciones miVentana = new VentanaOperaciones(5);
			miVentana.setVisible(true);
			dispose();
		}
		if(ubicacion.equals("Dificil")) 
		{
			VentanaOperaciones miVentana = new VentanaOperaciones(6);
			miVentana.setVisible(true);
			dispose();
		}
	}
}

