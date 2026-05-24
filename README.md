# Lights Out
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![WinForms](https://img.shields.io/badge/WinForms-net8.0--windows-blue)
> **Trabajo práctico universitario** — originalmente en **Java Swing** para la materia *Programación 3*, migrado a **.NET 8 WinForms**.
El clásico juego *Lights Out*: un tablero de luces que se encienden y apagan. El objetivo es **apagar todas las luces** presionando las celdas correctas.
---
## Capturas de pantalla
<table>
  <tr>
    <td><img width="497" alt="Menú principal" src="https://github.com/user-attachments/assets/dea642aa-f99f-4ea0-9dea-7ecc2daec8e5"></td>
    <td><img width="616" alt="Tablero Clásico" src="https://github.com/user-attachments/assets/67a0d5b0-911c-4506-96f7-95bd71d14fd6"></td>
  </tr>
  <tr>
    <td><img width="619" alt="Tablero Noche" src="https://github.com/user-attachments/assets/ef936f0e-b912-4543-b46e-5ba8c742453f"></td>
    <td><img width="845" alt="Estadísticas" src="https://github.com/user-attachments/assets/5ef33cb5-bb36-444a-bb9f-e6d3634ad35e"></td>
  </tr>
</table>

---
## Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows (WinForms)
## Comandos útiles
| Acción | Comando |
|---|---|
| Ejecutar el juego | `dotnet run --project src\LightsOut` |
| Ejecutar tests | `dotnet test` |
| Compilar | `dotnet build` |
| Compilar (Release) | `dotnet build -c Release` |
## Cómo se juega
El tablero tiene luces que pueden estar **encendidas** (oscuras) o **apagadas** (claras). Al presionar una celda se invierte el estado de toda su **fila** y toda su **columna**.
> Victoria: todas las celdas apagadas.
### Dificultades
| Botón | Tamaño |
|---|---|
| Fácil | 4×4 |
| Intermedio | 5×5 |
| Difícil | 6×6 |
| Personalizado | 3×3 a 8×8 |
## Características
- **4 esquemas de color**: Clásico, Noche, Fuego, Hielo
- **Pista**: revela un movimiento óptimo por turno (solver O(n²) por paridad)
- **Sonido**: tonos WAV generados en memoria, volumen ajustable y mute
- **Estadísticas**: persistencia en JSON por tamaño de tablero (récord, partidas jugadas/ganadas)
- **Feedback visual**: resaltado de fila/columna al presionar
- **Animación de victoria**: barrido diagonal dorado al ganar
## Arquitectura
```
src/LightsOut/
├── Program.cs                   ← entry point [STAThread]
├── Forms/                       ← pantallas (menú, juego, estadísticas)
├── Controls/                    ← controles reutilizables (tablero, botones)
├── Model/Tablero.cs             ← lógica del juego + solver
├── Data/EstadisticasRepository.cs ← persistencia CSV/JSON
└── Sound/GeneradorSonido.cs     ← sonido WAV en memoria
```
Sin dependencias NuGet externas.
## Tests
17 tests (xUnit) sobre el modelo (`Tablero`) y la persistencia (`EstadisticasRepository`). Sin tests de UI.
```powershell
dotnet test
```
## Autor
**Pablo Igei Nakagawa** — [@PabloIgeiNakagawa](https://github.com/PabloIgeiNakagawa)
