package operaciones;

public class procesosLightOut {
	private boolean[][] tabla;
	private int turnos;
	private int recordTurnos;
	
	public procesosLightOut(int tamano) 
	{
		tabla = new boolean[tamano][tamano];
	}
	
	public void random() 
	{
		for(int fila = 0; fila<tabla.length; fila++) 
		{
			for(int columna = 0; columna <tabla[0].length; columna++) 
			{
				if(Math.random()>0.5) 
				{
					tabla[fila][columna] = true;
				}
			}
			
		}
	}
	
	public void presionar(int fila, int columna) 
	{
		turnos++;
		for(int columnasVecinas = 0; columnasVecinas < tabla.length; columnasVecinas++)
		{
			
			tabla[fila][columnasVecinas]=!tabla[fila][columnasVecinas];
		}
		
		for(int filasVecinas = 0; filasVecinas < tabla.length; filasVecinas++) if(filasVecinas != fila)
		{
			tabla[filasVecinas][columna]=!tabla[filasVecinas][columna];
		}
	}
	
	public boolean juegoTerminado() 
	{
		for(int fila = 0; fila < tabla.length; fila++) 
		{
			for(int columna = 0; columna < tabla[0].length; columna++) 
			{
				if(tabla[fila][columna]) 
				{
					return false;
				}
			}
		}
		
		if(recordTurnos == 0) 
		{
			recordTurnos = turnos;
		}
		else 
		{
			recordTurnos = Math.min(recordTurnos, turnos);
		}

		turnos = 0;
	
		return true;
	}
	
	public boolean estaEncendidaLuz(int fila, int columna) 
	{
		return tabla[fila][columna];
	}

	public int turnosRealizados() 
	{
		return turnos;
	}
	
	public int recordTurnos() 
	{
		return recordTurnos;
	}
		
}

