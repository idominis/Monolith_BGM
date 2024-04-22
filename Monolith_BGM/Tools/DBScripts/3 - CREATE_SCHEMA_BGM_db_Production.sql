USE BGM_db
GO

/****** Object:  Schema [Production]    Script Date: 22.4.2024. 11:20:31 ******/
CREATE SCHEMA [Production]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Contains objects related to products, inventory, and manufacturing.' , @level0type=N'SCHEMA',@level0name=N'Production'
GO


