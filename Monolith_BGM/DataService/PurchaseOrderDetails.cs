using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

// --------------------------------------------------------------------------------
/// <summary>
/// 
/// </summary>
// --------------------------------------------------------------------------------
[XmlRoot("PurchaseOrderDetails")]
public class PurchaseOrderDetails
{
    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    /// <value>
    /// The details.
    /// </value>
    [XmlElement("PurchaseOrderDetail")]
    public List<PurchaseOrderDetailDto>? Details { get; set; }
}
