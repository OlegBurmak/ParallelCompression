using System;
using System.IO;

namespace GzipStreamTest
{
    public class BaseCompressed
    {
        public int CorsCount { get; }
        //Static buffer size
        public int PortionSize { get; }
        public FileInfo FileInfo { get; set; }
        public FileStream TargetStream { get; set; }
        public FileHelper FileHelper { get; set; }

        public BaseCompressed()
        {
            CorsCount = Environment.ProcessorCount;
            PortionSize = (2048*2048);
        }

        internal virtual void AddFile(string input, string output)
        {
            using(FileStream stream = File.Create(output))
            {
                FileInfo = new FileInfo(input);
                TargetStream = stream;
                FileHelper = new FileHelper(TargetStream);
                CheckFileSize(FileInfo);
            }
        }

        internal virtual void CheckFileSize(FileInfo fileI)
        {
            
        }
    }
}