using Monolith_BGM.DataAccess.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class XmlService
{
    // Define the method as generic with a type parameter T
    public T LoadFromXml<T>(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            return (T)serializer.Deserialize(stream);
        }
    }

    public void GenerateXMLFiles(List<PurchaseOrderSummary> summaries)
    {
        var serializer = new XmlSerializer(typeof(List<PurchaseOrderSummary>));

        // Build the base path dynamically to include the current user's Documents folder
        string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                       "BGM_project", "RebexTinySftpServer-Binaries-Latest", "data", "Xmls_Created");

        // Ensure the directory exists
        Directory.CreateDirectory(basePath);

        foreach (var group in summaries.GroupBy(s => s.PurchaseOrderID))
        {
            string fileName = Path.Combine(basePath, $"PurchaseOrderGenerated_{group.Key}.xml");
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(stream, group.ToList());
            }
        }
    }


}
