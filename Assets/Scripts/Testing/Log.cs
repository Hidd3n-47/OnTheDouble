using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public static class Log 
{
    static readonly string file = "log.txt";
    public static void Msg(string msg)
    {
        Debug.Log(msg);

        msg += "\n";

        FileStream fs = File.Open(Log.file,FileMode.Append);
        fs.Write(Encoding.Unicode.GetBytes(msg), 0, Encoding.Unicode.GetByteCount(msg));
        fs.Close();
    }
}
