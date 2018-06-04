using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.IO;

public class LoggerTest {

	[Test]
	public void TestLogWriterCreation() {
        LogWriter logger = new LogWriter();
        logger.Start("TestLog");
        logger.Stop();

        Assert.IsTrue(Path.Combine("Logs", "TestLog.log").Equals(logger.FilePath), string.Format("Actual Filepath is {0}", logger.FilePath));

        logger = new LogWriter();
        logger.Start();
        logger.Stop();

        Assert.IsTrue(logger.FilePath.Contains("logger"), string.Format("Actual Filepath is {0}", logger.FilePath));
	}

    [Test]
    public void TestLogWriting()
    {
        string filename = "TestLog";
        string filepath = Path.Combine("Logs", string.Format("{0}.log", filename));
        File.Delete(filepath);

        LogWriter logger = new LogWriter();
        logger.Start(filename);

        logger.Log("This is a test log entry");
        for (int i = 0; i < 10; i++)
        {
            logger.Log("{0}\t{1}\t{2}", 2, i, 10999);
        }

        logger.Stop();

        string[] lines = File.ReadAllLines(filepath);

        string expected = string.Format("{0}\t{1}\t{2}", 2, 4, 10999);
        Assert.IsTrue(lines[5].Contains(expected), 
            string.Format("Actual: {0} | Expected: {1}", lines[5], expected));
    }
}
