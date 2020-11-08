using System;

namespace GzipStreamTest
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length == 0)
            {
                System.Console.WriteLine("Программа запущена некоректно");
                return 1;
            }

            ParamData param = new ParamData(args);
            if(param.Validation())
            {   
                try
                {
                    switch(param.Method)
                    {
                        case "compress":
                            ParallelCompressed compressed = new ParallelCompressed(param.Input, param.Output);
                            break;
                        case "decompress":
                            ParallelDecompressed decompressed = new ParallelDecompressed(param.Input, param.Output);
                            break;
                        default :
                            System.Console.WriteLine("Enter Incorrect method");
                            return 1;
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return 1;
                }     
            }
            else
            {
                return 1;
            }
            
            
            System.Console.WriteLine("Success");
            return 0;

        }

    }
}
