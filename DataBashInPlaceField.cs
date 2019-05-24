using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DataBash
{
    class DataBashInPlaceField
    {
        Dictionary<string, string> definedReplacements;
        List<int> ignoreColumns;
        Random r;

        public int Seed
        {
            get { return m_Seed; }
            set { m_Seed = value; r = new Random(m_Seed); }
        }
        private int m_Seed;

        public DataBashInPlaceField()
        {
            definedReplacements = new Dictionary<string, string>();
            ignoreColumns = new List<int>();
            Seed = DateTime.Now.Millisecond;
        }

        public void ClearIgnoreColumns()
        {
            ignoreColumns.Clear();
        }

        public int SetIgnoreColumns(string input, string divider)
        {
            string[] roughFields = Regex.Split(input, divider);
            for (int i = 0; i < roughFields.Length; i++)
            {
                string f = roughFields[i];
                if (f.Contains("PRESERVEDATA"))
                {
                    ignoreColumns.Add(i);
                    //Console.WriteLine("ignoreColumns.Added " + i + f);
                }
            }
            return ignoreColumns.Count;
        }

            public string BashTextInPlace(string input, string divider, string EOL, int startRow)
        {
            char quote = '\"';

            string output = string.Empty;
            bool inQuote = false;
            string thisField = string.Empty;
            int currentColumn = 0, row=0;
            foreach (char c in input)
            {
                bool inEOL = EOL.Contains(c.ToString());
                if (c == quote)
                {
                    inQuote = !inQuote;
                    thisField += c;
                }
                else if ((c == divider[0]||inEOL) && !inQuote)
                {
                    // end field/column
                    string newText = row<startRow ? thisField : ReturnBashedText(currentColumn, thisField);
                    output += newText + c;
                    thisField = string.Empty;
                    currentColumn = inEOL ? 0 : currentColumn + 1;
                    if (inEOL)
                    {
                        // advance row count
                        row++;
                        if (row % 100 == 0)
                        {
                            System.Console.Write(".");
                        }
                    }
                }
                else
                {
                    thisField += c;
                }
            }
            // last field
            string lastBashedField = ReturnBashedText(currentColumn, thisField);
            output += lastBashedField;

            return output;
        }

        private string ReturnBashedText(int currentColumn, string thisField)
        {
            string newField;
            if (ignoreColumns.Contains(currentColumn))
            {
                newField = thisField;
                //Console.WriteLine("ignoreColumns.Contains " + currentColumn);
            }
            else
            {
                definedReplacements.TryGetValue(thisField, out newField);
                if (newField == null)
                {
                    newField = BashFieldInPlace(thisField);
                    definedReplacements.Add(thisField, newField);
                }
            }
            return newField;
        }

        public string BashFieldInPlace(string input)
        {
            string output = String.Empty;
            foreach (char c in input)
            {
                output += ChangeCharacter(c, r, input.Contains("@"));
            }
            return output;
        }

        public char ChangeCharacter(char c, Random r, bool reuseTranslations)
        {
            if (reuseTranslations)
            {
                string newField;
                definedReplacements.TryGetValue(c.ToString(), out newField);
                if (newField != null)
                {
                    return newField[0];
                }
            }

             char newC;
            if (Char.IsLower(c))
            {
                int lowerA = 'a';
                int lowerZ = 'z';
                int newChar = r.Next(lowerA, lowerZ);
                newC = (char)newChar;
            }
            else if (Char.IsUpper(c))
            {
                int A = 'A';
                int Z = 'Z';
                int newChar = r.Next(A, Z);
                newC = (char)newChar;
            }
            else if (Char.IsNumber(c))
            {
                int newChar = r.Next(9);
                newC = newChar.ToString()[0];
            }
            else
            {
                newC = c;
            }

            if (reuseTranslations)
            {
                definedReplacements.Add(c.ToString(), newC.ToString());
            }

            return newC;
        }
    }
}
