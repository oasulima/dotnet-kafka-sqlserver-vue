SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[AddInternalInventoryItem]
    @Id varchar(100),
    @Version int,
    @Symbol varchar(10),
    @Price decimal(10, 5),
    @Quantity int,
    @SoldQuantity int,
    @Source varchar(max),
    @CreatingType varchar(40),
    @Tag varchar(max),
    @CoveredInvItemId varchar(100),
    @Status varchar(20),
    @CreatedAt datetime2
AS
INSERT INTO [dbo].[InternalInventoryItems] (Id,
                                                 Version,
                                                 Symbol,
                                                 Price,
                                                 Quantity,
                                                 SoldQuantity,
                                                 Source,
                                                 CreatingType,
                                                 Tag,
                                                 CoveredInvItemId,
                                                 Status,
                                                 CreatedAt)
VALUES (@Id,
        @Version,
        @Symbol,
        @Price,
        @Quantity,
        @SoldQuantity,
        @Source,
        @CreatingType,
        @Tag,
        @CoveredInvItemId,
        @Status,
        @CreatedAt);
GO
