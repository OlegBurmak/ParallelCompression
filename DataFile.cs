using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GzipStreamTest
{
    public class DataFile
    {

        public string Name { get; }
        public string Extension { get; }
        public long FileLength { get; }
        public long EndFileInfoPosition { get; }
        public int BlockCount { get; }
        public int[] BlockLength { get; }


        public DataFile(string name, string extension,long fileLength, long endFileInfoPosition, int blockCount, int[] blockLength)
        {
            Name = name;
            Extension = extension;
            FileLength = fileLength;
            EndFileInfoPosition = endFileInfoPosition;
            BlockCount = blockCount;
            BlockLength = blockLength;
        }

        public DataFile(string name, int blockCount, string extension)
        {
            Name = name;
            BlockCount = blockCount;
            Extension = extension;
        }

        
    }

    public class FileHelper
    {
        public Stream Stream { get; }

        public FileHelper(Stream stream)
        {
            Stream = stream;
        }

        public void AddFileInfo(DataFile dataFile)
        {
            var fileNameByte = Encoding.UTF8.GetBytes(dataFile.Name);
            var fileExtensionByte = Encoding.UTF8.GetBytes(dataFile.Extension);
            //Write a length name to file
            Stream.Write(BitConverter.GetBytes(fileNameByte.Length), 0, 4);
            //Write file name to file
            Stream.Write(fileNameByte, 0, fileNameByte.Length);
            //Write block count to file
            Stream.Write(BitConverter.GetBytes(dataFile.BlockCount), 0, 4);
            //Write a length file extension to file
            Stream.Write(BitConverter.GetBytes(fileExtensionByte.Length), 0, 4);
            //Write file extension
            Stream.Write(fileExtensionByte, 0, fileExtensionByte.Length);
            //Write last stream position
            Stream.Write(BitConverter.GetBytes(Stream.Position + 4), 0, 4);
        }

        public DataFile GetDataFiles()
        {
            byte[] buffer;
            DataFile titleFiles = null;
            while (Stream.Position < Stream.Length)
            {
                long positionInTheStream;
                var blockCount = 0;
                var blockLength = new int[0];
                long fileLength = 0;
                buffer = new byte[4];
                Stream.Read(buffer, 0, 4);
                var filePathLength = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[filePathLength];
                Stream.Read(buffer, 0, buffer.Length);
                var name = Encoding.UTF8.GetString(buffer);
                buffer = new byte[4];
                Stream.Read(buffer, 0, 4);
                blockCount = BitConverter.ToInt32(buffer, 0);
                Stream.Read(buffer, 0, 4);
                var fileExtensionByteLength = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[fileExtensionByteLength];
                Stream.Read(buffer, 0, buffer.Length);
                var fileExtension = Encoding.UTF8.GetString(buffer);
                buffer = new byte[4];
                Stream.Read(buffer, 0, 4);
                positionInTheStream = BitConverter.ToInt32(buffer, 0);
                blockLength = new int[blockCount];
                long pos = positionInTheStream;
                for (int i = 0; i < blockCount; i++)
                {
                    Stream.Read(buffer, 0, 4);
                    blockLength[i] = BitConverter.ToInt32(buffer, 0);
                    fileLength += blockLength[i];
                    pos += (blockLength[i] + 4);
                    Stream.Position = pos;
                }
                titleFiles = new DataFile(name,fileExtension,fileLength,positionInTheStream,blockCount,blockLength);
            }
            return titleFiles;
        }

    }
}