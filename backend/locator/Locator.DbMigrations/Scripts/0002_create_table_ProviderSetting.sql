SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderSetting](
	[ProviderId] [varchar](100) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Discount] [decimal](10, 5) NOT NULL,
	[Active] [bit] NOT NULL,
	[BuyRequestTopic] [varchar](max) NULL,
	[BuyResponseTopic] [varchar](max) NULL,
	[QuoteRequestTopic] [varchar](max) NULL,
	[QuoteResponseTopic] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[ProviderSetting] ADD  CONSTRAINT [PK_ProviderSetting] PRIMARY KEY CLUSTERED 
(
	[ProviderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[ProviderSetting] ADD  CONSTRAINT [AK_ProviderSetting] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
