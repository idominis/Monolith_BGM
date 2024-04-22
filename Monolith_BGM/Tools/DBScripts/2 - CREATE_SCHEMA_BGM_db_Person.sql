USE BGM_db
GO

/****** Object:  Schema [Person]    Script Date: 22.4.2024. 11:23:27 ******/
CREATE SCHEMA [Person]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Contains objects related to names and addresses of customers, vendors, and employees' , @level0type=N'SCHEMA',@level0name=N'Person'
GO


