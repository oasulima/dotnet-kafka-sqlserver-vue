SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInternalInventoryItemsHistory] @Take int,
                                                         @Symbol varchar(10) = NULL,
                                                         @ProviderId varchar(10) = NULL,
                                                         @BeforeCreatedAt datetime2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    WITH CTE_PageRecordIds as (SELECT TOP (@Take) items.[Id],
                                                  MAX(items.[Version])   as [MaxVersion],
                                                  MAX(items.[CreatedAt]) as [MaxTime]
                               FROM [dbo].[InternalInventoryItems] items with (nolock)
                               where (@Symbol is NULL or items.[Symbol] = @Symbol)
                                 and (@BeforeCreatedAt is NULL or items.[CreatedAt] <= @BeforeCreatedAt)
                                 and (@ProviderId is NULL or items.[Source] = @ProviderId)
                               group by items.[Id]
                               order by [MaxTime] desc)

    SELECT items.[Id],
           items.[Version],
           items.[Symbol],
           items.[Price],
           items.[Quantity],
           items.[SoldQuantity],
           items.[Source],
           items.[CreatingType],
           items.[Tag],
           items.[CoveredInvItemId],
           items.[Status],
           items.[CreatedAt],
           items.[Timestamp]
    FROM [dbo].[InternalInventoryItems] items with (nolock)
    WHERE EXISTS(SELECT 1
                 FROM CTE_PageRecordIds
                 WHERE CTE_PageRecordIds.[Id] = items.[Id]
                   AND CTE_PageRecordIds.MaxVersion = items.[Version])
    order by CreatedAt desc

END
GO
