SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[AddQuoteDetails]
    @QuoteId varchar(100),
    @ResponseDetailsJson ntext
AS
BEGIN
    INSERT INTO [dbo].[QuoteDetails] ([QuoteId], [ResponseDetailsJson])
    VALUES (@QuoteId, @ResponseDetailsJson)
END;
GO
