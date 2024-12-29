SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetQuoteDetails]
    @QuoteId varchar(100)
AS
BEGIN
	SELECT [QuoteId], [ResponseDetailsJson]
	FROM [dbo].[QuoteDetails] t with (nolock)
	WHERE t.[QuoteId] = @QuoteId;
END;
GO
