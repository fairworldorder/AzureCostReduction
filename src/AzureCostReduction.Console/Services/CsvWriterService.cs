using CsvHelper;
using System.Collections;
using System.Globalization;

namespace AzureCostReduction.Console.Services
{
    public class CsvWriterService
    {
        public void Write(string filename, IEnumerable records)
        {
            using (var writer = new StreamWriter($"C:\\Temp\\{filename}"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                csv.WriteRecords(records);
        }
    }
}