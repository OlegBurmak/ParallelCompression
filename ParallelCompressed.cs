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
        FileInfo outInfo;
        public ParallelCompressed(string input, string output)
        {
            AddFile(input, output);
        }

        internal override void AddFile(string input, string output)
        {
            outInfo = new FileInfo(output);
            if(outInfo.Extension == "")
            {
                outInfo = new FileInfo(outInfo.FullName + ".gz");
            }
            using(FileStream stream = outInfo.Create())
            {
                FileInfo = new FileInfo(input);
                TargetStream = stream;
                FileHelper = new FileHelper(TargetStream);
                CheckFileSize(FileInfo);
            }
        }

        internal override void CheckFileSize(FileInfo fileI)
        {
            if(fileI.Length > 1000000)
            {
                System.Console.WriteLine("Start of compression");
            }
            else
            {
                System.Console.WriteLine("Start of compression, but since the file is small, it is possible to increase the size of the archive in relation to the original file");
            }
            CompressFile(fileI);
        }


        private void CompressFile(FileInfo fileI)
        {

            var blockCount = GetBlockCount(fileI.Length);
            var blockSizeArray = GetBlockSizeArray(fileI.Length, blockCount);
            FileHelper.AddFileInfo(new DataFile(fileI.Name, blockCount*CorsCount, fileI.Extension));
            
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
                        //Perform parallel compression, the result is written to the task array
                        resultData[j] = Task.Run(() => CompressBlock(bytes));
                    }

                    Task.WaitAll(resultData);
                    WriteToFile(resultData.ToArray());
                }
            }
            System.Console.WriteLine($"End of compression OutFile: {outInfo.FullName}");
        }

        private void WriteToFile(Task<byte[]>[] data)
        {
            var result = data.Select(r => r.Result).ToArray();
            foreach(var res in result)
            {
                //Write block size info
                TargetStream.Write(BitConverter.GetBytes(res.Length), 0, 4);
                TargetStream.Write(res, 0, res.Length);
            }
        }

        private byte[] CompressBlock(byte[] data)
        {
            using(var compressedStream = new MemoryStream())
            {
                using(var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
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

        //Get the correct number of blocks for parallel execution
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