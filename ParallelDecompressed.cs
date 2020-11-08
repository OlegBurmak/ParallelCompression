using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace GzipStreamTest
{
    public class ParallelDecompressed : BaseCompressed
    {
        private FileInfo FileOutputInfo;
        public ParallelDecompressed(string input, string output)
        {
            AddFile(input, output);
        }

        internal override void AddFile(string input, string output)
        {
            FileInfo = new FileInfo(input);
            FileOutputInfo = new FileInfo(output);
            CheckFileSize(FileInfo);
        }

        internal override void CheckFileSize(FileInfo fileI)
        {
            DecompressFile(fileI, FileOutputInfo);
        }

        private void DecompressFile(FileInfo fileI, FileInfo outFileI)
        {
            using(FileStream sourceStream = fileI.OpenRead())
            {
                DataFile Data = null;
                try
                {
                    FileHelper = new FileHelper(sourceStream);
                    Data = FileHelper.GetDataFiles();
                    if(outFileI.Extension == "")
                    {
                        outFileI = new FileInfo(outFileI.FullName + Data.Extension);
                        System.Console.WriteLine(outFileI.FullName);
                    }
                    using(FileStream targetStream = outFileI.Create())
                    {
                        TargetStream = targetStream;
                        byte[] data;
                        long pozition = Data.EndFileInfoPosition + 4;
                        foreach(var item in Data.BlockLength)
                        {
                            sourceStream.Position = pozition;
                            data = new byte[item];
                            sourceStream.Read(data, 0, data.Length);
                            pozition += item + 4;
                            DecompressBlock(data);
                        }
                    }
                    System.Console.WriteLine($"End of compression OutFile: {outFileI.FullName}");
                }
                catch (System.Exception)
                {
                    throw new Exception("Unable to decompress this file.");
                }
                
                
                
                //TODO Parallel file decompression
                // foreach(var item in blockSizeArray)
                // {
                //     resultData = new Task<byte[]>[item.Length];
                //     for(int j = 0; j < item.Length; j++)
                //     {
                //         sourceStream.Position = pozition;
                //         data = new byte[item[j]];
                //         sourceStream.Read(data, 0, data.Length);
                //         pozition += item[j] + 4;
                //         resultData[j] = Task.Run(() => DecompressBlock(data));
                //     }
                //     Task.WaitAll(resultData);
                //     WriteToFile(resultData.Select(r => r.Result).ToArray());
                // }
            }
        }


        private void DecompressBlock(byte[] bytes)
        {
            using(var decompressedStream = new MemoryStream(bytes))
            {
                using(GZipStream stream = new GZipStream(decompressedStream, CompressionMode.Decompress))
                {
                    stream.CopyTo(TargetStream);
                }   
            }
            
        }
    }
}