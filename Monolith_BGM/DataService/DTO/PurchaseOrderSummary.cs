﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monolith_BGM.DataAccess.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class PurchaseOrderSummary
    {
        /// <summary>
        /// Gets or sets the purchase order identifier.
        /// </summary>
        /// <value>
        /// The purchase order identifier.
        /// </value>
        public int PurchaseOrderID { get; set; }
        /// <summary>
        /// Gets or sets the purchase order detail identifier.
        /// </summary>
        /// <value>
        /// The purchase order detail identifier.
        /// </value>
        public int PurchaseOrderDetailID { get; set; }
        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        /// <value>
        /// The order date.
        /// </value>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Gets or sets the vendor identifier.
        /// </summary>
        /// <value>
        /// The vendor identifier.
        /// </value>
        public int VendorID { get; set; }
        /// <summary>
        /// Gets or sets the name of the vendor.
        /// </summary>
        /// <value>
        /// The name of the vendor.
        /// </value>
        public string VendorName { get; set; }
        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public int ProductID { get; set; }
        /// <summary>
        /// Gets or sets the product number.
        /// </summary>
        /// <value>
        /// The product number.
        /// </value>
        public string ProductNumber { get; set; }
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName { get; set; }
        /// <summary>
        /// Gets or sets the order qty.
        /// </summary>
        /// <value>
        /// The order qty.
        /// </value>
        public int OrderQty { get; set; }
        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        /// <value>
        /// The unit price.
        /// </value>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Gets or sets the line total.
        /// </summary>
        /// <value>
        /// The line total.
        /// </value>
        public decimal LineTotal { get; set; }
        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        /// <value>
        /// The sub total.
        /// </value>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Gets or sets the tax amt.
        /// </summary>
        /// <value>
        /// The tax amt.
        /// </value>
        public decimal TaxAmt { get; set; }
        /// <summary>
        /// Gets or sets the freight.
        /// </summary>
        /// <value>
        /// The freight.
        /// </value>
        public decimal Freight { get; set; }
        /// <summary>
        /// Gets or sets the total due.
        /// </summary>
        /// <value>
        /// The total due.
        /// </value>
        public decimal TotalDue { get; set; }
    }

}
