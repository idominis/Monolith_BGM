using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Src;
using Monolith_BGM.XMLService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class XmlService : IXmlService
{
    private FileManager _fileManager;

    // Inject FileManager through constructor
    public XmlService(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    // Define the method as generic with a type parameter T
    public T LoadFromXml<T>(string filePath)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new ApplicationException($"Error deserializing file {filePath}: {ex.Message}", ex);
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
