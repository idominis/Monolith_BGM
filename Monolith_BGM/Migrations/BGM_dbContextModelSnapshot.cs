﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Monolith_BGM.Models;

#nullable disable

namespace Monolith_BGM.Migrations
{
    [DbContext(typeof(BGM_dbContext))]
    partial class BGM_dbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Monolith_BGM.Models.PurchaseOrderDetail", b =>
                {
                    b.Property<int>("PurchaseOrderId")
                        .HasColumnType("int")
                        .HasColumnName("PurchaseOrderID")
                        .HasComment("Primary key. Foreign key to PurchaseOrderHeader.PurchaseOrderID.");

                    b.Property<int>("PurchaseOrderDetailId")
                        .HasColumnType("int")
                        .HasColumnName("PurchaseOrderDetailID")
                        .HasComment("Primary key. One line number per purchased product.");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime")
                        .HasComment("Date the product is expected to be received.");

                    b.Property<decimal>("LineTotal")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("money")
                        .HasComputedColumnSql("(isnull([OrderQty]*[UnitPrice],(0.00)))", false)
                        .HasComment("Per product subtotal. Computed as OrderQty * UnitPrice.");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())")
                        .HasComment("Date and time the record was last updated.");

                    b.Property<short>("OrderQty")
                        .HasColumnType("smallint")
                        .HasComment("Quantity ordered.");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductID")
                        .HasComment("Product identification number. Foreign key to Product.ProductID.");

                    b.Property<decimal>("ReceivedQty")
                        .HasColumnType("decimal(8,2)")
                        .HasComment("Quantity actually received from the vendor.");

                    b.Property<decimal>("RejectedQty")
                        .HasColumnType("decimal(8,2)")
                        .HasComment("Quantity rejected during inspection.");

                    b.Property<decimal>("StockedQty")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("decimal(9,2)")
                        .HasComputedColumnSql("(isnull([ReceivedQty]-[RejectedQty],(0.00)))", false)
                        .HasComment("Quantity accepted into inventory. Computed as ReceivedQty - RejectedQty.");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("money")
                        .HasComment("Vendor's selling price of a single product.");

                    b.HasKey("PurchaseOrderId", "PurchaseOrderDetailId")
                        .HasName("PK_PurchaseOrderDetail_PurchaseOrderID_PurchaseOrderDetailID");

                    b.ToTable("PurchaseOrderDetail", "Purchasing");

                    b.HasComment("Individual products associated with a specific purchase order. See PurchaseOrderHeader.");
                });

            modelBuilder.Entity("Monolith_BGM.Models.PurchaseOrderHeader", b =>
                {
                    b.Property<int>("PurchaseOrderId")
                        .HasColumnType("int")
                        .HasColumnName("PurchaseOrderID")
                        .HasComment("Primary key.");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int")
                        .HasColumnName("EmployeeID")
                        .HasComment("Employee who created the purchase order. Foreign key to Employee.BusinessEntityID.");

                    b.Property<decimal>("Freight")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("money")
                        .HasDefaultValueSql("((0.00))")
                        .HasComment("Shipping cost.");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())")
                        .HasComment("Date and time the record was last updated.");

                    b.Property<DateTime>("OrderDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())")
                        .HasComment("Purchase order creation date.");

                    b.Property<byte>("RevisionNumber")
                        .HasColumnType("tinyint")
                        .HasComment("Incremental number to track changes to the purchase order over time.");

                    b.Property<DateTime?>("ShipDate")
                        .HasColumnType("datetime")
                        .HasComment("Estimated shipment date from the vendor.");

                    b.Property<int>("ShipMethodId")
                        .HasColumnType("int")
                        .HasColumnName("ShipMethodID")
                        .HasComment("Shipping method. Foreign key to ShipMethod.ShipMethodID.");

                    b.Property<byte>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValueSql("((1))")
                        .HasComment("Order current status. 1 = Pending; 2 = Approved; 3 = Rejected; 4 = Complete");

                    b.Property<decimal>("SubTotal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("money")
                        .HasDefaultValueSql("((0.00))")
                        .HasComment("Purchase order subtotal. Computed as SUM(PurchaseOrderDetail.LineTotal)for the appropriate PurchaseOrderID.");

                    b.Property<decimal>("TaxAmt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("money")
                        .HasDefaultValueSql("((0.00))")
                        .HasComment("Tax amount.");

                    b.Property<decimal>("TotalDue")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("money")
                        .HasComputedColumnSql("(isnull(([SubTotal]+[TaxAmt])+[Freight],(0)))", true)
                        .HasComment("Total due to vendor. Computed as Subtotal + TaxAmt + Freight.");

                    b.Property<int>("VendorId")
                        .HasColumnType("int")
                        .HasColumnName("VendorID")
                        .HasComment("Vendor with whom the purchase order is placed. Foreign key to Vendor.BusinessEntityID.");

                    b.HasKey("PurchaseOrderId")
                        .HasName("PK_PurchaseOrderHeader_PurchaseOrderID");

                    b.ToTable("PurchaseOrderHeader", "Purchasing");

                    b.HasComment("General purchase order information. See PurchaseOrderDetail.");
                });

            modelBuilder.Entity("Monolith_BGM.Models.PurchaseOrderDetail", b =>
                {
                    b.HasOne("Monolith_BGM.Models.PurchaseOrderHeader", "PurchaseOrderHeader")
                        .WithMany("PurchaseOrderDetails")
                        .HasForeignKey("PurchaseOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PurchaseOrderHeader");
                });

            modelBuilder.Entity("Monolith_BGM.Models.PurchaseOrderHeader", b =>
                {
                    b.Navigation("PurchaseOrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
