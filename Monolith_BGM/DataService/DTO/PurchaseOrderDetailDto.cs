using FluentValidation;
using Monolith_BGM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Monolith_BGM.DataAccess.DTO
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("PurchaseOrderDetail")]
    public partial class PurchaseOrderDetailDto
    {
        /// <summary>
        /// Primary key. Foreign key to PurchaseOrderHeader.PurchaseOrderID.
        /// </summary>
        /// <value>
        /// The purchase order identifier.
        /// </value>
        [XmlAttribute("PurchaseOrderID")]
        public int PurchaseOrderId { get; set; }
        /// <summary>
        /// Primary key. One line number per purchased product.
        /// </summary>
        /// <value>
        /// The purchase order detail identifier.
        /// </value>
        [XmlAttribute("PurchaseOrderDetailID")]
        public int PurchaseOrderDetailId { get; set; }
        //public int XmlPurchaseOrderDetailId { get; set; }
        /// <summary>
        /// Date the product is expected to be received.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        [XmlElement("DueDate")]
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Quantity ordered.
        /// </summary>
        /// <value>
        /// The order qty.
        /// </value>
        [XmlElement("OrderQty")]
        public short OrderQty { get; set; }
        /// <summary>
        /// Product identification number. Foreign key to Product.ProductID.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        [XmlElement("ProductID")]
        public int ProductId { get; set; }
        /// <summary>
        /// Vendor's selling price of a single product.
        /// </summary>
        /// <value>
        /// The unit price.
        /// </value>
        [XmlElement("UnitPrice")]
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Per product subtotal. Computed as OrderQty * UnitPrice.
        /// </summary>
        /// <value>
        /// The line total.
        /// </value>
        [XmlElement("LineTotal")]
        public decimal LineTotal { get; set; }
        /// <summary>
        /// Quantity actually received from the vendor.
        /// </summary>
        /// <value>
        /// The received qty.
        /// </value>
        [XmlElement("ReceivedQty")]
        public decimal ReceivedQty { get; set; }
        /// <summary>
        /// Quantity rejected during inspection.
        /// </summary>
        /// <value>
        /// The rejected qty.
        /// </value>
        [XmlElement("RejectedQty")]
        public decimal RejectedQty { get; set; }
        /// <summary>
        /// Quantity accepted into inventory. Computed as ReceivedQty - RejectedQty.
        /// </summary>
        /// <value>
        /// The stocked qty.
        /// </value>
        [XmlElement("StockedQty")]
        public decimal StockedQty { get; set; }
        /// <summary>
        /// Date and time the record was last updated.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        [XmlElement("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Monolith_BGM.DataAccess.DTO.PurchaseOrderDetailDto&gt;" />
    public class PurchaseOrderDetailValidator : AbstractValidator<PurchaseOrderDetailDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderDetailValidator"/> class.
        /// </summary>
        public PurchaseOrderDetailValidator()
        {
            RuleFor(x => x.DueDate).NotEmpty().WithMessage("Due date is required.");
            RuleFor(x => x.PurchaseOrderDetailId).NotEmpty().WithMessage("PurchaseOrderDetailId is required.");
            RuleFor(x => x.PurchaseOrderId).NotEmpty().WithMessage("PurchaseOrderId is required.");
        }
    }
}
