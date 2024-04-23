using Monolith_BGM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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
    //public PurchaseOrderDetails LoadFromXml(string filePath)
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(PurchaseOrderDetails));
    //    using (FileStream stream = new FileStream(filePath, FileMode.Open))
    //    {
    //        var purchaseOrderDetails = (PurchaseOrderDetails)serializer.Deserialize(stream);
     
    //        foreach (var detail in purchaseOrderDetails)
    //        {
    //            detail.XmlPurchaseOrderDetailId = detail.PurchaseOrderDetailId;
    //        }

    //        return purchaseOrderDetails;
    //    }
    //}
}


