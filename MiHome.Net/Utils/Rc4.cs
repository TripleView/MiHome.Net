namespace MiHome.Net.Utils;

public class Rc4
{
    private int idx = 0, jdx = 0;
    private List<byte> ksa = new List<byte>();

    public Rc4(string key)
    {
        var pwd = Convert.FromBase64String(key);
        var cnt= pwd.Length;
        var tempKsa = new List<byte>();
        for (int i = 0; i < 256; i++)
        {
            tempKsa.Add((byte)i);
        }

        var j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = (j + tempKsa[i] + pwd[i % cnt]) & 255;
            var temp = tempKsa[i];
            tempKsa[i] = tempKsa[j];
            tempKsa[j] = temp;
        }

        ksa = tempKsa;
        idx = 0;
        jdx = 0;
    }

    public byte[] Crypt(byte[] data)
    {
        var ksa = this.ksa;
        var i = idx;
        var j = jdx;
        var outList=new List<byte>();
        foreach (byte byt in data)
        {
            i = (i + 1) & 255;
            j = (j + ksa[i]) & 255;
            var temp = ksa[i];
            ksa[i] = ksa[j];
            ksa[j] = temp;
            var r = (byte)(byt ^ ksa[(ksa[i] + ksa[j]) & 255]);
            outList.Add(r);
        }

        idx = i;
        jdx = j;
        this.ksa= ksa;
        return outList.ToArray();
    }

    public Rc4 Init1024()
    {
         Crypt(new byte[1024]);
         return this;
    }
}