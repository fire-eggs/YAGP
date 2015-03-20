using System;
using System.IO;
using SharpGEDParser;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
//            string fpath = args[0];
            //            new FileRead().ReadGed(fpath);

            string apath = @"E:\test geds 2";
            var files = Directory.GetFiles(apath, "*.ged");
            foreach (var afile in files)
            {
                Console.WriteLine(afile);
                new FileRead().ReadGed(afile);
            }
        }
    }
}
