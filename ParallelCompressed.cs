using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace GzipStreamTest
{
    public class ParallelCompressed : BaseCompressed
    {
        private FileInfo fileInfo;
        private FileStream targetStream;
        public ParallelCompressed(string input, string output)
        {
            AddFile(input, output);
        }

        private void AddFile(string input, string output)
        {
            using(FileStream stream = File.Create(output))
            {
                fileInfo = new FileInfo(input);
                targetStream = stream;
                CheckFileSize(fileInfo);
                System.Console.WriteLine("Success");
            }
        }

        private void CheckFileSize(FileInfo fileI)
        {
            if(fileI.Length > 6000000)
            {
                CompressFile(fileI);
            }
        }


        private void CompressFile(FileInfo fileI)
        {
            var blockCount = GetBlockCount(fileI.Length);
            var blockSizeArray = GetBlockSizeArray(fileI.Length, blockCount);
            
            using(FileStream sourceStream = fileI.OpenRead())
            {
                
                for(int i = 0; i < blockCount; i++)
                {
                    long[] data = GetBlockSizeArray(blockSizeArray[i], CorsCount);
                    Task<byte[]>[] resultData = new Task<byte[]>[data.Length];

                    for(int j = 0; j < data.Length; j++)
                    {
                        var bytes = new byte[data[j]];
                        sourceStream.Read(bytes, 0, bytes.Length);
                        resultData[j] = Task.Run(() => CompressBlock(bytes));
                    }

                    Task.WaitAll(resultData);
                    WriteToFile(resultData.ToArray());
                }


            }
        }

        private void WriteToFile(Task<byte[]>[] data)
        {
            var result = data.Select(r => r.Result).ToArray();
            foreach(var res in result)
            {
                targetStream.Write(res, 0, res.Length);
                System.Console.WriteLine("Woop");
            }
        }

        private byte[] CompressBlock(byte[] data)
        {
            using(var compressedStream = new MemoryStream())
            {
                using(var zipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
                {
                    zipStream.Write(data, 0, data.Length);
                    zipStream.Close();
                    return compressedStream.ToArray();
                }
            }
        }

        //Get an array of block size by formula [(file size/portion size)].
        private long[] GetBlockSizeArray(long fileLength, int blockCount)
        {
            long[] blocks = Enumerable.Range(0, blockCount).Select(x => fileLength/blockCount).ToArray();
            if(blocks.Sum() != fileLength)
            {
                blocks[blocks.Length - 1] += (fileLength - blocks.Sum());
            }
            return blocks;
        }

        private int GetBlockCount(long fileLength)
        {
            var result = (float)((fileLength/PortionSize)/CorsCount);
            if(result % 2 == 0 || result < 1)
            {
                result += 1;
            }
            return (int)result;
        }



    }
}