namespace LightsOut.Controls;

public class BotonJuego : Button
{
    public BotonJuego(string text)
    {
        Text = text;
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 1;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI", 11, FontStyle.Regular);
        UseVisualStyleBackColor = true;
    }
}
