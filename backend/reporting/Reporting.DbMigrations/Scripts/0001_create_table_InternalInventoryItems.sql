SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InternalInventoryItems](
	[Id] [varchar](100) NOT NULL,
	[Version] [int] NOT NULL,
	[Symbol] [varchar](10) NOT NULL,
	[Price] [decimal](10, 5) NOT NULL,
	[Quantity] [int] NOT NULL,
	[SoldQuantity] [int] NOT NULL,
	[Source] [varchar](max) NOT NULL,
	[Tag] [varchar](max) NULL,
	[Status] [varchar](20) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[CreatingType] [varchar](40) NOT NULL,
	[CoveredInvItemId] [varchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[InternalInventoryItems] ADD  CONSTRAINT [PK_InternalInventoryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [idx_symbol_createdat] ON [dbo].[InternalInventoryItems]
(
	[Symbol] ASC,
	[CreatedAt] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InternalInventoryItems] ADD  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO
