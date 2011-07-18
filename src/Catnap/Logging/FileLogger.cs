using System;
using System.IO;

namespace Catnap.Common.Logging
{
    public class FileLogger : ILogger
    {
        private const int fileSizeLimitBytes = 100000;
        private readonly string fileName;
        private string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private string filePath;

        public FileLogger(string fileName)
        {
            this.fileName = fileName;
            filePath = Path.Combine(targetFolder, fileName);
        }

        public void LogMessage(string message)
        {
            CreateFileIfNotExists();

            StreamWriter writer = null;
            try
            {
                writer = File.AppendText(filePath);
                writer.WriteLine("{0} : {1}", DateTime.Now, message);
            }
            finally
            {
                if (writer != null) writer.Close();
            }

            RollFillIfOverSizeLimit();
        }

        //TODO: enhance to roll more than one
        private void RollFillIfOverSizeLimit()
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length < fileSizeLimitBytes)
            {
                return;
            }
            var newFileName = "Old_" + Path.GetFileName(filePath);
            newFileName = Path.Combine(targetFolder, newFileName);
            File.Copy(filePath, newFileName, true);
            DeleteLogFile();
        }

        private void CreateFileIfNotExists()
        {
            if (File.Exists(filePath))
            {
                return;
            }
            FileStream fileStream = null;
            try
            {
                fileStream = File.Create(filePath);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public void DeleteLogFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}