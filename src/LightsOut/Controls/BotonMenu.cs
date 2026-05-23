namespace LightsOut.Controls;

public class BotonMenu : Button
{
    public BotonMenu(string text)
    {
        Text = text;
        FlatStyle = FlatStyle.Flat;
        var fa = FlatAppearance;
        fa.BorderSize = 0;
        fa.MouseOverBackColor = Color.FromArgb(0x1D, 0x4E, 0xD8);
        BackColor = Color.FromArgb(0x25, 0x63, 0xEB);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 16, FontStyle.Bold);
        Cursor = Cursors.Hand;
        Size = new Size(200, 38);
    }
}
