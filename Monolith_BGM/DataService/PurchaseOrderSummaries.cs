using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("ArrayOfPurchaseOrderSummary")]
public class PurchaseOrderSummaries
{
    [XmlElement("PurchaseOrderSummary")]
    public List<PurchaseOrderSummary> Summaries { get; set; }
}

