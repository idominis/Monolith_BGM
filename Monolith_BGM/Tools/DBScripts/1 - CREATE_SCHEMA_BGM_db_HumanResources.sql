USE BGM_db
GO

/****** Object:  Schema [HumanResources]    Script Date: 22.4.2024. 11:24:03 ******/
CREATE SCHEMA [HumanResources]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Contains objects related to employees and departments.' , @level0type=N'SCHEMA',@level0name=N'HumanResources'
GO


