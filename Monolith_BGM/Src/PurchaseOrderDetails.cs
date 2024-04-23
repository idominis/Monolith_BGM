using Monolith_BGM.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("PurchaseOrderDetails")]
public class PurchaseOrderDetails
{
    [XmlElement("PurchaseOrderDetail")]
    public List<PurchaseOrderDetail> Details { get; set; }
}
