using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Src;
using Monolith_BGM.XMLService;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;

public class XmlService : IXmlService
{
    private FileManager _fileManager;

    public XmlService(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public T LoadFromXml<T>(string filePath) where T : class
    {
        XmlSchemaSet schemas = new XmlSchemaSet();
        schemas.Add("", "C:\\Users\\ido\\source\\repos\\Monolith_BGM\\Monolith_BGM\\Tools\\purchaseOrderDetails.xsd");

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ValidationType = ValidationType.Schema;
        settings.Schemas = schemas;
        settings.ValidationEventHandler += ValidationEventHandler; // This will now log instead of throwing

        try
        {
            using (var stream = File.OpenRead(filePath))
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read()) { } // Validate the XML structure against the schema
            }

            using (var stream = File.OpenRead(filePath))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream); // Deserialize if the file is structurally correct
            }

        }
        catch (XmlException xmlEx)
        {
            Log.Error($"XML format error in {filePath}: {xmlEx.Message}");
            return null; // Return null or default(T) to indicate an issue with this specific file
        }
        catch (InvalidOperationException ex) when (ex.InnerException is FormatException)
        {
            Log.Error($"Schema validation error in {filePath}: {ex.InnerException.Message}");
            return null; // Return null or default(T) for handling downstream
        }
        catch (Exception ex)
        {
            Log.Error($"General error processing file {filePath}: {ex.Message}");
            return null; // General error fallback
        }
    }

    private static void ValidationEventHandler([NotNull] object sender, ValidationEventArgs e)
    {
        //if (sender == null) throw new ArgumentNullException(nameof(sender));

        if (e.Severity == XmlSeverityType.Error || e.Severity == XmlSeverityType.Warning)
        {
            Log.Warning($"Validation warning/error: {e.Message}"); // Log the warning/error instead of throwing an exception
        }
    }

    public void GenerateXMLFiles(List<PurchaseOrderSummary> summaries, DateTime? startDate = null, DateTime? endDate = null)
    {
        var serializer = new XmlSerializer(typeof(List<PurchaseOrderSummary>));

        string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();

        // Ensure the directory exists
        Directory.CreateDirectory(localBaseDirectoryXmlCreatedPath);

        if (startDate.HasValue && endDate.HasValue)
        {
            // If date range is given, generate a single XML file for that range
            string dateRangeFileName = Path.Combine(localBaseDirectoryXmlCreatedPath, $"PurchaseOrderSummariesGenerated_{startDate.Value:yyyyMMdd}_to_{endDate.Value:yyyyMMdd}.xml");
            using (var stream = new FileStream(dateRangeFileName, FileMode.Create))
            {
                serializer.Serialize(stream, summaries);
            }
        }
        else
        {
            // Generate multiple XML files, one per PurchaseOrderID
            foreach (var group in summaries.GroupBy(s => s.PurchaseOrderID))
            {
                string fileName = Path.Combine(localBaseDirectoryXmlCreatedPath, $"PurchaseOrderGenerated_{group.Key}.xml");
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    serializer.Serialize(stream, group.ToList());
                }
            }
        }
    }

    public List<int> ExtractPurchaseOrderIdsFromXml(string filePath)
    {
        try
        {
            var summaries = LoadFromXml<PurchaseOrderSummaries>(filePath);
            return summaries.Summaries.Select(s => s.PurchaseOrderID).Distinct().ToList();
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Failed to extract Purchase Order IDs.", ex);
        }
    }

    public List<int> ExtractPurchaseOrderDetailIdsFromXml(string filePath)
    {
        try
        {
            var summaries = LoadFromXml<PurchaseOrderSummaries>(filePath);
            return summaries.Summaries.Select(s => s.PurchaseOrderDetailID).Distinct().ToList();
        }
        catch (ApplicationException ex)
        {
            throw new ApplicationException("Failed to extract Purchase Order Detail IDs.", ex);
        }
    }
}
