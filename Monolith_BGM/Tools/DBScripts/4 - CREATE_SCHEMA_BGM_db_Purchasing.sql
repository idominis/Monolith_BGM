USE BGM_db
GO

/****** Object:  Schema [Purchasing]    Script Date: 20.4.2024. 22:53:40 ******/
CREATE SCHEMA [Purchasing]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Contains objects related to vendors and purchase orders.' , @level0type=N'SCHEMA',@level0name=N'Purchasing'
GO


