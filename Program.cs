using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBash
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0 || args[0] == "?")
            {
                System.Console.WriteLine("DataBash is a simple tool to randomize text files while maintaining the same structure.");
                System.Console.WriteLine("version 0.2 (May 4 2019)");
                System.Console.WriteLine("tech@danbachmann.com");
            }

            if (args.Length<2 || args.Length> 4)
            {
                System.Console.WriteLine("Usage: DataBash inputfilename outputfilename <ignoreFirstLine> <configurationFileName>");
                System.Console.WriteLine("Example: DataBash test.csv testOutput.csv");
                System.Console.WriteLine("Example: DataBash test.csv testOutput.csv false");
                System.Console.WriteLine("Example: DataBash test.csv testOutput.csv true testConfig.csv");
                return;
            }


            DataBashInPlaceField db = new DataBashInPlaceField();
            bool ignoreFirstLine = true;
            if (args.Length > 2)
            {
                try
                {
                    ignoreFirstLine = Convert.ToBoolean(args[2]);
                }
                catch 
                {
                    System.Console.WriteLine("could not read ignoreFirstLine value. default is true");
                }
            }
          
            string inputFileName = args[0];
            string outputFileName = args[1];
            string configFileName = args.Length>3 ? args[3] : string.Empty;

            int startLine = ignoreFirstLine ? 1 : 0;
            string divider = ",", EOL="\r\n";

            DataBashFile dbf = new DataBashFile();
            int linesWritten = 0;
            try
            {
                linesWritten = dbf.ProcessFile(inputFileName, outputFileName, configFileName,
                    startLine, divider, EOL);
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            System.Console.WriteLine(linesWritten>0 ? "processed " + inputFileName + " " + configFileName + " into " + outputFileName : "Not processed: " + inputFileName);

        }
    }
}
