using System.Runtime.InteropServices;

namespace LightsOut.Sound;

public static class GeneradorSonido
{
    public static float Volumen { get; set; } = 0.5f;

    public static void Click() { if (Volumen > 0) ReproducirTono(600, 60); }

    public static void Victoria()
    {
        if (Volumen <= 0) return;
        new Thread(() =>
        {
            int[] freqs = [523, 659, 784, 1047];
            foreach (int f in freqs)
            {
                if (Volumen <= 0) break;
                ReproducirTono(f, 130);
                Thread.Sleep(90);
            }
        }).Start();
    }

    public static void Pista() { if (Volumen > 0) ReproducirTono(880, 100); }

    public static void Error() { if (Volumen > 0) ReproducirTono(200, 200); }

    private static void ReproducirTono(int frecuenciaHz, int duracionMs)
    {
        try
        {
            int sampleRate = 8000;
            int nSamples = sampleRate * duracionMs / 1000;
            var buffer = new byte[nSamples];

            for (int i = 0; i < nSamples; i++)
            {
                double angulo = 2.0 * Math.PI * frecuenciaHz * i / sampleRate;
                buffer[i] = (byte)(Math.Sin(angulo) * 80 * Volumen + 128);
            }

            using var ms = new MemoryStream(44 + nSamples);
            var bw = new BinaryWriter(ms);
            bw.Write("RIFF"u8);
            bw.Write(36 + nSamples);
            bw.Write("WAVE"u8);
            bw.Write("fmt "u8);
            bw.Write(16);
            bw.Write((short)1);          // PCM
            bw.Write((short)1);          // mono
            bw.Write(sampleRate);
            bw.Write(sampleRate);
            bw.Write((short)1);          // blockAlign
            bw.Write((short)8);          // bitsPerSample
            bw.Write("data"u8);
            bw.Write(nSamples);
            bw.Write(buffer);

            ms.Position = 0;
            using var player = new System.Media.SoundPlayer(ms);
            player.PlaySync();
        }
        catch
        {
        }
    }
}
