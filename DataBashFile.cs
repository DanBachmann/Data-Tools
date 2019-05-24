namespace DataBash
{
    class DataBashFile
    {
        public int ProcessFile(string inputFileName, string outputFileName,
            string configurationFile,
            int startLine, string divider, string EOL)
        {
            if (!System.IO.File.Exists(inputFileName))
            {
                return 0;
            }

            DataBashInPlaceField db = new DataBashInPlaceField();
            if (configurationFile.Length > 0)
            {
                System.Console.WriteLine("reading config " + configurationFile);
                string[] inputConfigurationLines = System.IO.File.ReadAllLines(configurationFile);
            
                int ignoreColumnCount = db.SetIgnoreColumns(inputConfigurationLines[inputConfigurationLines.Length-1], divider);
                System.Console.WriteLine("ignoring " + ignoreColumnCount + " columns");
            }

            System.Console.WriteLine("reading " + inputFileName);
            string inputF = System.IO.File.ReadAllText(inputFileName);
            System.Console.Write("processing " + inputF.Length + " characters ");
            string outputF = db.BashTextInPlace(inputF, divider, EOL, startLine);
            System.Console.WriteLine();

            System.Console.WriteLine("writiing " + outputFileName +"; " + outputF.Length + " characters");
            System.IO.File.WriteAllText(outputFileName, outputF);
            return outputF.Length;
        }
    }
}
