using System;

namespace GzipStreamTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFile = "D://win7.iso"; // исходный файл
            string compressedFile = "D://Books/test4.gz"; // сжатый файл
            //string targetFile = "D://Books/book_new.pdf"; // восстановленный файл

            var timestart = DateTime.Now;
            ParallelCompressed compresser = new ParallelCompressed(sourceFile, compressedFile);
            var timeEnd = DateTime.Now;

            System.Console.WriteLine($"Start = {timestart} - End = {timeEnd}");

        }

    }
}
