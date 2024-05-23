USE [BGM_db]
GO

/****** Object:  Table [HumanResources].[Employee]    Script Date: 22.4.2024. 21:43:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [HumanResources].[Employee](
	[BusinessEntityID] [int] NOT NULL,
	[NationalIDNumber] [nvarchar](15) NOT NULL,
	[LoginID] [nvarchar](256) NOT NULL,
	[OrganizationNode] [hierarchyid] NULL,
	[OrganizationLevel]  AS ([OrganizationNode].[GetLevel]()),
	[JobTitle] [nvarchar](50) NOT NULL,
	[BirthDate] [date] NOT NULL,
	[MaritalStatus] [nchar](1) NOT NULL,
	[Gender] [nchar](1) NOT NULL,
	[HireDate] [date] NOT NULL,
	[SalariedFlag] [dbo].[Flag] NOT NULL,
	[VacationHours] [smallint] NOT NULL,
	[SickLeaveHours] [smallint] NOT NULL,
	[CurrentFlag] [dbo].[Flag] NOT NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Employee_BusinessEntityID] PRIMARY KEY CLUSTERED 
(
	[BusinessEntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [DF_Employee_SalariedFlag]  DEFAULT ((1)) FOR [SalariedFlag]
GO

ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [DF_Employee_VacationHours]  DEFAULT ((0)) FOR [VacationHours]
GO

ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [DF_Employee_SickLeaveHours]  DEFAULT ((0)) FOR [SickLeaveHours]
GO

ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [DF_Employee_CurrentFlag]  DEFAULT ((1)) FOR [CurrentFlag]
GO

ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [DF_Employee_rowguid]  DEFAULT (newid()) FOR [rowguid]
GO

ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [DF_Employee_ModifiedDate]  DEFAULT (getdate()) FOR [ModifiedDate]
GO



