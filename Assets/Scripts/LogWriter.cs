using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

public class LogWriter
{
    private class Entry
    {
        DateTime ts;
        string msg;

        public Entry(DateTime timestamp, string message)
        {
            ts = timestamp;
            msg = message.Replace("\r", "|").Replace("\n", "|");
        }

        public void WriteToFile(string filepath)
        {
            var tsStr = ts.ToString();
            var line = string.Format("{0}\t{1}", ts.Ticks, msg);

            using (var streamWriter = new StreamWriter(filepath, true))
            {
                streamWriter.WriteLine(line);
            }

        }
    }

    public static int id =0;
    public string FilePath { get; private set; }

    private string baseFolder = "Logs";

    private object lockObj = new object();
    private Queue<Entry> entries = new Queue<Entry>();
    private bool isRunning = false;
    private Thread thread;

    public LogWriter(string filename=null)
    {
        id++;

        string name = string.IsNullOrEmpty(filename) ? string.Format("logger_{0}-{1}.log", id, DateTime.Now.ToString("s")).Replace(":", "") : FilePath = string.Format("{0}.log", filename);
        FilePath = Path.Combine(baseFolder, name);
    }
    
    ~LogWriter()
    {
        id--;
    }

    private void Run()
    {
        while (true)
        {
            lock (lockObj)
            {
                if (entries.Count == 0)
                {
                    if (!isRunning) break;
                }
                else
                {
                    try
                    {
                        var entry = entries.Dequeue();
                        entry.WriteToFile(FilePath);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.Log(string.Format("Logger {0} failed. Stoping Logger. Error: {1}", id, ex));
                        isRunning = false;
                    }
                }
            }
            Thread.Sleep(10);
        }
    }

    public void Start()
    {
        lock (lockObj)
        {
            isRunning = true;
        }
        thread = new Thread(Run);
        thread.Start();
    }

    public void Stop()
    {
        lock (lockObj)
        {
            isRunning = false;
        }
        thread.Join();
        thread = null;
    }

    public bool Log(string message)
    {
        lock (lockObj)
        {
            if (!isRunning)
                return false;
            var entry = new Entry(DateTime.Now, message);
            entries.Enqueue(entry);
        }
        return true;
    }

    public bool Log(object obj)
    {
        return Log(obj.ToString());
    }

    public bool Log(string format, params object[] args)
    {
        return Log(string.Format(format, args));
    }
}
