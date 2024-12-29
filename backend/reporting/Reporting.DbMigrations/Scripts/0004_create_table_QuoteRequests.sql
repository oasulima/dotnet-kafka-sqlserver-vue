SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuoteRequests](
	[Id] [varchar](100) NOT NULL,
	[AccountId] [varchar](100) NOT NULL,
	[RequestType] [varchar](20) NOT NULL,
	[Symbol] [varchar](100) NULL,
	[Quantity] [int] NOT NULL,
	[AllowPartial] [bit] NOT NULL,
	[AutoApprove] [bit] NOT NULL,
	[MaxPriceForAutoApprove] [decimal](38, 8) NOT NULL,
	[Time] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[QuoteRequests] ADD  CONSTRAINT [PK_QuoteRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[RequestType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
