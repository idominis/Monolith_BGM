using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monolith_BGM.Migrations
{
    public partial class UpdateRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Purchasing");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderHeader",
                schema: "Purchasing",
                columns: table => new
                {
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: false, comment: "Primary key."),
                    RevisionNumber = table.Column<byte>(type: "tinyint", nullable: false, comment: "Incremental number to track changes to the purchase order over time."),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "((1))", comment: "Order current status. 1 = Pending; 2 = Approved; 3 = Rejected; 4 = Complete"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false, comment: "Employee who created the purchase order. Foreign key to Employee.BusinessEntityID."),
                    VendorID = table.Column<int>(type: "int", nullable: false, comment: "Vendor with whom the purchase order is placed. Foreign key to Vendor.BusinessEntityID."),
                    ShipMethodID = table.Column<int>(type: "int", nullable: false, comment: "Shipping method. Foreign key to ShipMethod.ShipMethodID."),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "Purchase order creation date."),
                    ShipDate = table.Column<DateTime>(type: "datetime", nullable: true, comment: "Estimated shipment date from the vendor."),
                    SubTotal = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))", comment: "Purchase order subtotal. Computed as SUM(PurchaseOrderDetail.LineTotal)for the appropriate PurchaseOrderID."),
                    TaxAmt = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))", comment: "Tax amount."),
                    Freight = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))", comment: "Shipping cost."),
                    TotalDue = table.Column<decimal>(type: "money", nullable: false, computedColumnSql: "(isnull(([SubTotal]+[TaxAmt])+[Freight],(0)))", stored: true, comment: "Total due to vendor. Computed as Subtotal + TaxAmt + Freight."),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "Date and time the record was last updated.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderHeader_PurchaseOrderID", x => x.PurchaseOrderID);
                },
                comment: "General purchase order information. See PurchaseOrderDetail.");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetail",
                schema: "Purchasing",
                columns: table => new
                {
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: false, comment: "Primary key. Foreign key to PurchaseOrderHeader.PurchaseOrderID."),
                    PurchaseOrderDetailID = table.Column<int>(type: "int", nullable: false, comment: "Primary key. One line number per purchased product."),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false, comment: "Date the product is expected to be received."),
                    OrderQty = table.Column<short>(type: "smallint", nullable: false, comment: "Quantity ordered."),
                    ProductID = table.Column<int>(type: "int", nullable: false, comment: "Product identification number. Foreign key to Product.ProductID."),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false, comment: "Vendor's selling price of a single product."),
                    LineTotal = table.Column<decimal>(type: "money", nullable: false, computedColumnSql: "(isnull([OrderQty]*[UnitPrice],(0.00)))", stored: false, comment: "Per product subtotal. Computed as OrderQty * UnitPrice."),
                    ReceivedQty = table.Column<decimal>(type: "decimal(8,2)", nullable: false, comment: "Quantity actually received from the vendor."),
                    RejectedQty = table.Column<decimal>(type: "decimal(8,2)", nullable: false, comment: "Quantity rejected during inspection."),
                    StockedQty = table.Column<decimal>(type: "decimal(9,2)", nullable: false, computedColumnSql: "(isnull([ReceivedQty]-[RejectedQty],(0.00)))", stored: false, comment: "Quantity accepted into inventory. Computed as ReceivedQty - RejectedQty."),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "Date and time the record was last updated.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetail_PurchaseOrderID_PurchaseOrderDetailID", x => new { x.PurchaseOrderID, x.PurchaseOrderDetailID });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetail_PurchaseOrderHeader_PurchaseOrderID",
                        column: x => x.PurchaseOrderID,
                        principalSchema: "Purchasing",
                        principalTable: "PurchaseOrderHeader",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Individual products associated with a specific purchase order. See PurchaseOrderHeader.");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderDetail",
                schema: "Purchasing");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHeader",
                schema: "Purchasing");
        }
    }
}
