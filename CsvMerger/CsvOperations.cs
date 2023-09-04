using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public class CsvOperations
{
    public static void MergeCsvFiles(string folderPath)
    {
        // Initialize Stopwatch for timing
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Initialize variables to hold statistics
        long totalRows = 0;
        double totalFileSizeMB = 0;
        string? firstFileHeader = null;

        // Create output file name and path
        string outputFileName = Path.GetFileName(folderPath) ?? "default";
        outputFileName += "_merged.csv";
        string outputPath = Path.Combine(folderPath, outputFileName);

        StringBuilder logBuilder = new StringBuilder();

        logBuilder.AppendLine($"Searching for CSV files in {folderPath}...");

        string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

        // Inform the user about the number of CSV files found
        logBuilder.AppendLine($"{csvFiles.Length} CSV files found in the provided directory.");

        if (csvFiles.Length == 0)
        {
            logBuilder.AppendLine("No CSV files found in the provided directory.");
            Console.WriteLine(logBuilder);
            return;
        }

        using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8))
        {
            for (int i = 0; i < csvFiles.Length; i++)
            {
                string filePath = csvFiles[i];

                if (filePath.Equals(outputPath, StringComparison.OrdinalIgnoreCase))
                {
                    // Skip the output file to avoid reading it
                    continue;
                }

                long rowCount = 0;
                long fileSizeBytes = new FileInfo(filePath).Length;
                double fileSizeKB = fileSizeBytes / 1024.0;
                double fileSizeMB = fileSizeBytes / (1024.0 * 1024.0);

                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (rowCount == 0)
                        {
                            if (firstFileHeader == null)
                            {
                                firstFileHeader = line;
                            }
                            else if (!HeadersMatch(firstFileHeader, line))
                            {
                                logBuilder.AppendLine($"Warning: Header mismatch detected in file {filePath}");
                            }
                        }

                        if (i > 0 && rowCount == 0)
                        {
                            rowCount++;
                            continue;
                        }

                        writer.WriteLine(line);
                        rowCount++;
                    }
                }

                totalRows += rowCount;
                totalFileSizeMB += fileSizeMB;

                logBuilder.AppendLine($"File Ingested: {filePath}");
                logBuilder.AppendLine($"Row Count: {rowCount}");
                logBuilder.AppendLine($"File Size: {fileSizeKB:F2} KB | {fileSizeMB:F2} MB");
                logBuilder.AppendLine($"Subtotal Rows: {totalRows}");
                logBuilder.AppendLine("-------------");
            }
        }

        // Stop the Stopwatch
        stopwatch.Stop();

        // Append final statistics and elapsed time
        logBuilder.AppendLine("Merging Completed!");
        logBuilder.AppendLine($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        logBuilder.AppendLine($"Final Output File: {outputPath}");
        logBuilder.AppendLine($"Final Row Count: {totalRows}");

        long finalFileSizeBytes = new FileInfo(outputPath).Length;
        double finalFileSizeKB = finalFileSizeBytes / 1024.0;
        double finalFileSizeMB = finalFileSizeBytes / (1024.0 * 1024.0);
        logBuilder.AppendLine($"Final File Size: {finalFileSizeKB:F2} KB | {finalFileSizeMB:F2} MB");

        // Output log
        Console.WriteLine(logBuilder.ToString());
    }

    private static bool HeadersMatch(string firstFileHeader, string? currentFileHeader)
    {
        return firstFileHeader.Equals(currentFileHeader, StringComparison.OrdinalIgnoreCase);
    }
}