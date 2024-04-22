using Monolith_BGM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("PurchaseOrderDetails")]
public class PurchaseOrderDetails
{
    [XmlElement("PurchaseOrderDetail")]
    public List<PurchaseOrderDetail> Details { get; set; }
}

// And adjust XmlDataLoader usage accordingly
public class XmlDataLoader
{
    public PurchaseOrderDetails LoadFromXml(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PurchaseOrderDetails));
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            return (PurchaseOrderDetails)serializer.Deserialize(stream);
        }
    }
}

// Usage
//var purchaseOrderDetails = xmlLoader.LoadFromXml(filePath);
//var purchaseOrders = purchaseOrderDetails.Details;

