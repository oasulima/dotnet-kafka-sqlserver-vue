SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddQuoteRequest]
    @Id                     varchar(100),
    @AccountId              varchar(100),
    @RequestType            varchar(20),
    @Symbol                 varchar(100),
    @Quantity               int,
    @AllowPartial           bit,
    @AutoApprove            bit,
    @MaxPriceForAutoApprove decimal(38,8),
    @Time datetime2
AS
INSERT INTO [dbo].[QuoteRequests]
([Id],
 [AccountId],
 [RequestType],
 [Symbol],
 [Quantity],
 [AllowPartial],
 [AutoApprove],
 [MaxPriceForAutoApprove],
 [Time])
VALUES
    (@Id,
     @AccountId,
     @RequestType,
     @Symbol,
     @Quantity,
     @AllowPartial,
     @AutoApprove,
     @MaxPriceForAutoApprove,
     @Time);
GO
