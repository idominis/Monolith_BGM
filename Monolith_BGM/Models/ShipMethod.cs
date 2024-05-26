using System;
using System.Collections.Generic;

namespace Monolith_BGM.Models
{
    /// <summary>
    /// Shipping company lookup table.
    /// </summary>
    public partial class ShipMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipMethod"/> class.
        /// </summary>
        public ShipMethod()
        {
            PurchaseOrderHeaders = new HashSet<PurchaseOrderHeader>();
        }

        /// <summary>
        /// Primary key for ShipMethod records.
        /// </summary>
        /// <value>
        /// The ship method identifier.
        /// </value>
        public int ShipMethodId { get; set; }
        /// <summary>
        /// Shipping company name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Minimum shipping charge.
        /// </summary>
        /// <value>
        /// The ship base.
        /// </value>
        public decimal ShipBase { get; set; }
        /// <summary>
        /// Shipping charge per pound.
        /// </summary>
        /// <value>
        /// The ship rate.
        /// </value>
        public decimal ShipRate { get; set; }
        /// <summary>
        /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
        /// </summary>
        /// <value>
        /// The rowguid.
        /// </value>
        public Guid Rowguid { get; set; }
        /// <summary>
        /// Date and time the record was last updated.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the purchase order headers.
        /// </summary>
        /// <value>
        /// The purchase order headers.
        /// </value>
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
    }
}
