SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountInventoryItems](
	[Id] [varchar](100) NOT NULL,
	[Version] [int] NOT NULL,
	[AccountId] [varchar](200) NOT NULL,
	[Symbol] [varchar](20) NOT NULL,
	[LocatedQuantity] [int] NOT NULL,
	[AvailableQuantity] [int] NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[AccountInventoryItems] ADD  CONSTRAINT [PK_AccountInventoryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccountInventoryItems] ADD  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO
