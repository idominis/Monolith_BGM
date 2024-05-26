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
    public partial class PurchaseOrderSentDto
    {
        /// <summary>
        /// Gets or sets the purchase order identifier.
        /// </summary>
        /// <value>
        /// The purchase order identifier.
        /// </value>
        public int PurchaseOrderId { get; set; }
        /// <summary>
        /// Gets or sets the purchase order detail identifier.
        /// </summary>
        /// <value>
        /// The purchase order detail identifier.
        /// </value>
        public int PurchaseOrderDetailId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [order processed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [order processed]; otherwise, <c>false</c>.
        /// </value>
        public bool OrderProcessed { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [order sent].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [order sent]; otherwise, <c>false</c>.
        /// </value>
        public bool OrderSent { get; set; }
        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public int Channel { get; set; }
        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime ModifiedDate { get; set; }
    }
}