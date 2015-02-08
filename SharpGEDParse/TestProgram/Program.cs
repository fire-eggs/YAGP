using SharpGEDParser;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            string fpath = args[0];

            new FileRead().ReadGed(fpath);
        }
    }
}
