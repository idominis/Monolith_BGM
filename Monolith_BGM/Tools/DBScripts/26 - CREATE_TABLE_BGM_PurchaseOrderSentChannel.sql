USE [BGM_db]
GO

/****** Object:  Table [Purchasing].[PurchaseOrderSentChannel]    Script Date: 22.4.2024. 21:48:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE Purchasing.PurchaseOrderSentChannel (
    PurchaseOrderSentChannelId [int] PRIMARY KEY IDENTITY(1,1),
    Name [nvarchar](50),
    Active [bit]
)

SET IDENTITY_INSERT [Purchasing].[PurchaseOrderSentChannel] ON 
INSERT INTO Purchasing.PurchaseOrderSentChannel (PurchaseOrderSentChannelId, Name, Active) VALUES
(0, 'Auto', 1),
(1, 'Custom', 1),
(2, 'e-mail', 1),
(3, 'Other', 1)
SET IDENTITY_INSERT [Purchasing].[PurchaseOrderSentChannel] OFF
GO