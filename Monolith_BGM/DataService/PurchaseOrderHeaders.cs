using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("PurchaseOrderHeaders")]
public class PurchaseOrderHeaders
{
    [XmlElement("PurchaseOrderHeader")]
    public List<PurchaseOrderHeaderDto> Headers { get; set; }
}
