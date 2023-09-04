using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Taking folder path as an argument
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a folder path as an argument.");
            return;
        }

        string folderPath = args[0];

        if (Directory.Exists(folderPath))
        {
            CsvOperations.MergeCsvFiles(folderPath);
        }
        else
        {
            Console.WriteLine("The specified folder path does not exist.");
        }
        
        Console.WriteLine("CsvMerger has completed its operation.");
    }
}