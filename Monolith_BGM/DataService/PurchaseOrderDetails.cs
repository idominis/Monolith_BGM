using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("PurchaseOrderDetails")]
public class PurchaseOrderDetails
{
    [XmlElement("PurchaseOrderDetail")]
    public List<PurchaseOrderDetailDto> Details { get; set; }
}
