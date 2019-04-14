using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Collections;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection c = new SQLiteConnection("Data Source=C:\\Users\\prais\\AVPIoO.db");
            c.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = c;
            DataTable dt = c.GetSchema("Tables");
            Dictionary<string, Dictionary<string, object>> dataBaseRules = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, Dictionary<string, string>> references = new Dictionary<string, Dictionary<string, string>>();
            foreach(DataRow d in dt.Rows)
            {
                
                try
                {
                    Dictionary<string, object> columns = new Dictionary<string, object>();
                    int ruleStart = (d[6] as string).IndexOf('(');
                    string rules = "";
                    for(int i = ruleStart; i< (d[6] as string).Length; i++)
                    {
                        rules += (d[6] as string)[i];
                    }
                    rules = rules.Replace("(", string.Empty).Replace(")", string.Empty).Replace("\t", " ").Trim();
                    string[] atrRules1 = rules.Split('\n');
                    atrRules1 = atrRules1.Select(x => x.Trim()).ToArray();
                    Dictionary<string, string> keyTable = new Dictionary<string, string>();
                    foreach (string rule in atrRules1)
                    {
                        string columnName = "";
                        string columnType = "";

                        int rulesIndex = 1;
                        if (rule.StartsWith("\""))
                        {

                            for (int i = 1; rule[i] != '"'; i++)
                            {
                                columnName += rule[i];
                                rulesIndex++;
                            }
                            rulesIndex += 2;
                            for (int i = rulesIndex; rule[i] != ' '; i++)
                            {
                                columnType += rule[i];
                            }

                            object columnTypeForDataTable = new object();
                            switch (columnType)
                            {
                                case "INTEGER":
                                    columnTypeForDataTable = new Int32();
                                    columnTypeForDataTable = 0;
                                    break;
                                case "TEXT":
                                    columnTypeForDataTable = "Hey";
                                    break;
                                case "REAL":
                                    columnTypeForDataTable = new Double();
                                    columnTypeForDataTable = 1.24;
                                    break;
                            }
                            columns.Add(columnName, columnTypeForDataTable);
                            
                        } else if(rule.StartsWith("FOREIGN KEY"))
                        {
                            string tmp = "";
                            int keysIndex = 12;
                            for(int i = 12;rule[i] != '"'; i++)
                            {
                                tmp += rule[i];
                                keysIndex++;
                            }
                            string tmpTable = "";
                            keysIndex += 14;
                            for (int i = keysIndex; rule[i] != '"'; i++)
                            {
                                tmpTable += rule[i];
                            }
                            keyTable.Add(tmp, tmpTable);
                        }
                        /*
                        foreach (var pair in keyTable)
                        {
                            Console.Write(pair.Key);
                            Console.Write(": ");
                            Console.WriteLine(pair.Value);
                        }
                        Console.Write('\n');
                        */
                    }

                    dataBaseRules.Add((d[2] as string), columns);
                    references.Add((d[2] as string), keyTable);

                    
                }
                catch (Exception)
                {
                    continue;
                }
            }
            foreach (var pair in dataBaseRules)
            {
                Console.Write(pair.Key);
                Console.Write(": {");
                foreach (var sPair in pair.Value)
                {
                    Console.Write(sPair.Key);
                    Console.Write(": ");
                    Console.Write(sPair.Value);
                    Console.Write(", ");
                }
                Console.WriteLine("} ,");
            }
            Console.WriteLine();
            foreach (var pair in references)
            {
                Console.Write(pair.Key);
                Console.Write(": {");
                foreach (var sPair in pair.Value)
                {
                    Console.Write(sPair.Key);
                    Console.Write(": ");
                    Console.Write(sPair.Value);
                    Console.Write(", ");
                }
                Console.WriteLine("}");
            }
            
            Console.ReadKey();
        }
    }
}
