USE [BGM_db]
GO

/****** Object:  Table [Purchasing].[PurchaseOrdersSent]    Script Date: 22.4.2024. 21:48:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Purchasing].[PurchaseOrdersSent](
	[PurchaseOrderSentId] [int] PRIMARY KEY IDENTITY(1,1),
	[PurchaseOrderId] [int] NOT NULL,
	[OrderProcessed] [bit] NOT NULL,
	[OrderSent] [bit] NOT NULL,
	[Channel] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
);

