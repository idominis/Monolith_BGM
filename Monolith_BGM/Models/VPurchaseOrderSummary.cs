﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Monolith_BGM.Models
{
    public partial class VPurchaseOrderSummary
    {
        public int PurchaseOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int ProductId { get; set; }
        public string ProductNumber { get; set; }
        public string ProductName { get; set; }
        public short OrderQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal Freight { get; set; }
        public decimal TotalDue { get; set; }
    }
}