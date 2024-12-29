SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocatorQuoteResponses](
	[Id] [varchar](100) NOT NULL,
	[AccountId] [varchar](100) NOT NULL,
	[Symbol] [varchar](100) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[ErrorMessage] [varchar](max) NULL,
	[Time] [datetime] NOT NULL,
	[ReqQty] [int] NOT NULL,
	[FillQty] [int] NOT NULL,
	[Price] [decimal](10, 5) NOT NULL,
	[DiscountedPrice] [decimal](10, 5) NOT NULL,
	[Source] [varchar](max) NOT NULL,
	[Sources] [varchar](max) NOT NULL,
	[Fee]  AS ([Price]*[FillQty]) PERSISTED,
	[DiscountedFee]  AS ([DiscountedPrice]*[FillQty]) PERSISTED,
	[IsReportableStatus]  AS (case when [Status]<>(11) AND [Status]<>(9) AND [Status]<>(5) then (1) else (0) end) PERSISTED NOT NULL,
	[RecordId] [varchar](100) NOT NULL,
	[Profit]  AS (case when [Status]=(0) then (0) else ([Price]-[DiscountedPrice])*[FillQty] end) PERSISTED
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[LocatorQuoteResponses] ADD  CONSTRAINT [LocatorQuoteResponses_pk] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
CREATE NONCLUSTERED INDEX [nci_combo] ON [dbo].[LocatorQuoteResponses]
(
	[IsReportableStatus] ASC,
	[Time] ASC,
	[Symbol] ASC,
	[Status] ASC,
	[AccountId] ASC,
	[ReqQty] ASC,
	[FillQty] ASC,
	[Price] ASC,
	[DiscountedPrice] ASC,
	[Fee] ASC,
	[DiscountedFee] ASC,
	[Profit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [nci_time] ON [dbo].[LocatorQuoteResponses]
(
	[Time] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
