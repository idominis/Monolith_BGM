USE [BGM_db]
GO

/****** Object:  Table [Purchasing].[PurchaseOrdersProcessedSent]    Script Date: 22.4.2024. 21:48:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Purchasing].[PurchaseOrdersProcessedSent](
	[PurchaseOrderId] [int]  NOT NULL,
	[PurchaseOrderDetailId] [int] PRIMARY KEY NOT NULL,
	[OrderProcessed] [bit] NOT NULL,
	[OrderSent] [bit] NOT NULL,
	[Channel] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
);

