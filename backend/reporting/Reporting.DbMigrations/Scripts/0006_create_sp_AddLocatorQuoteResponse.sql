SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddLocatorQuoteResponse] @RecordId varchar(100),
                                                @Id varchar(100),
                                                @AccountId varchar(100),
                                                @Symbol varchar(100),
                                                @Status tinyint,
                                                @ErrorMessage varchar(max),
                                                @Time datetime,
                                                @ReqQty int,
                                                @FillQty int,
                                                @Price decimal(10, 5),
                                                @DiscountedPrice decimal(10, 5),
                                                @Source varchar(max),
                                                @Sources varchar(max)
AS
BEGIN
    INSERT INTO [dbo].[LocatorQuoteResponses]
    ([RecordId],
     [Id],
     [AccountId],
     [Symbol],
     [Status],
     [ErrorMessage],
     [Time],
     [ReqQty],
     [FillQty],
     [Price],
     [DiscountedPrice],
     [Source],
     [Sources])
    VALUES (@RecordId,
            @Id,
            @AccountId,
            @Symbol,
            @Status,
            @ErrorMessage,
            @Time,
            @ReqQty,
            @FillQty,
            @Price,
            @DiscountedPrice,
            @Source,
            @Sources)

END
GO
