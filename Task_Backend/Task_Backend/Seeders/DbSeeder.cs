using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Task_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Task_Backend.Utils;


namespace Task_Backend.Seeders
{
    public class DbSeeder
    {
        public static void SeedData(ModelContext context, string documentsPath, string documentItemsPath)
        {
            SeedDocuments(context, documentsPath);
            SeedDocumentItems(context, documentItemsPath);
        }

        private static void SeedDocuments(ModelContext context, string filePath)
        {
            if (!context.Documents.Any())
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ";"
                });

                csv.Context.TypeConverterCache.AddConverter<DateTime>(new DateConverter());
                var records = csv.GetRecords<Document>();

                foreach (var record in records)
                {
                    context.Documents.Add(new Document
                    {
                        // ID WILL AUTOGEN
                        City = record.City,
                        Date = record.Date,
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        Type = record.Type
                    });
                }
                context.SaveChanges();
            }
        }

        private static void SeedDocumentItems(ModelContext context, string filePath)
        {
            if (!context.DocumentItems.Any())
            {
                using var reader = new StreamReader(filePath);
                var csvConfig = new CsvConfiguration(new CultureInfo("pl-PL"))
                {
                    HasHeaderRecord = true,
                    Delimiter = ";",
                    HeaderValidated = null,
                    MissingFieldFound = null
                };
                using var csv = new CsvReader(reader, csvConfig);

                var records = csv.GetRecords<DocumentItem>();

                foreach (var record in records)
                {
                    context.DocumentItems.Add(new DocumentItem
                    {
                        DocumentId = record.DocumentId,
                        Ordinal = record.Ordinal,
                        Product = record.Product,
                        Quantity = record.Quantity,
                        Price = record.Price,
                        TaxRate = record.TaxRate
                    });
                }
                context.SaveChanges();
            }
        }


    }
}
