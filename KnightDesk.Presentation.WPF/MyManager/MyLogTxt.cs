using System.IO;
using System.Text;
using System;
using UnityEngine;

public class MyLogTxt
{
    public static void WriteLog(string data)
    {
        try
        {
            string timeLog = DateTime.Now.ToString("h:mm:ss tt");
            string result = string.Format("[Time:{0}] === ||{1}||", timeLog, data);
            
            int length = Application.dataPath.LastIndexOf("/");
            string path = Application.dataPath.Substring(0, length).Replace("/", "\\") + "\\log.txt";
            
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using StreamWriter streamWriter = new StreamWriter(stream);
                    streamWriter.WriteLine(result);
                    return;
                }
            }
            using FileStream fileStream = File.Create(path);
            byte[] bytes = new UTF8Encoding().GetBytes(result);
            fileStream.Write(bytes, 0, bytes.Length);
            byte[] array = new byte[1];
            fileStream.Write(array, 0, array.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}