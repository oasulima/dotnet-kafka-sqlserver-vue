SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetQuoteRequests]
    @From datetime2 = NULL,
    @To datetime2 = NULL
AS
SELECT  [Id],
        [AccountId],
        [RequestType],
        [Symbol],
        [Quantity],
        [AllowPartial],
        [AutoApprove],
        [MaxPriceForAutoApprove],
        [Time]
FROM [dbo].[QuoteRequests] req with (nolock)
WHERE (@From is NULL or req.[Time] >= @From)
  AND (@To is NULL or req.[Time] <= @To);
GO
