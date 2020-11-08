using System.IO;

namespace GzipStreamTest
{
    public class ParamData
    {
        private string[] Args { get; }
        public string Method { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public ParamData(string[] args)
        {
            Args = args;
            
        }

        public bool Validation()
        {
            if(ChekValid())
            {
                GetParamData();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ChekValid()
        {
            if(Args.Length == 3)
            {
                if(Args[0].Length > 0)
                {
                    var extention = Path.GetExtension(Args[1]);
                    if(extention != null && extention != "")
                    {
                        var dir = Path.GetDirectoryName(Args[2]);
                        if(dir != null && dir != "")
                        {
                            return true;
                        }
                        else
                        {
                            System.Console.WriteLine("Incorrect output directory or file name");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Incorrect input file name");
                    }
                }
                else
                {
                    System.Console.WriteLine("Incorrect method");
                }
            }
            return false;
            
        }

        private void GetParamData()
        {
            Method = Args[0].ToLower();
            Input = Args[1];
            Output = Args[2];
        }
    }
}