USE [BGM_db]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW Purchasing.vPurchaseOrderSummary AS
SELECT 
    POH.PurchaseOrderID, 
    POH.OrderDate, 
    POH.VendorID, 
    V.Name AS VendorName, 
    POD.ProductID, 
    P.ProductNumber, 
    P.Name AS ProductName, 
    POD.OrderQty, 
    POD.UnitPrice, 
    POD.LineTotal, 
    POH.SubTotal, 
    POH.TaxAmt, 
    POH.Freight, 
    POH.TotalDue
FROM 
    Purchasing.PurchaseOrderHeader AS POH
INNER JOIN 
    Purchasing.PurchaseOrderDetail AS POD ON POD.PurchaseOrderID = POH.PurchaseOrderID
INNER JOIN 
    Purchasing.Vendor AS V ON V.BusinessEntityID = POH.VendorID
INNER JOIN 
    Production.Product AS P ON P.ProductID = POD.ProductID;

