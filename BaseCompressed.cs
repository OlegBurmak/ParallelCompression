using System;

namespace GzipStreamTest
{
    public class BaseCompressed
    {
        public int CorsCount { get; }
        public int PortionSize { get; }

        public BaseCompressed()
        {
            CorsCount = Environment.ProcessorCount;
            PortionSize = (2048*2048);
        }
    }
}