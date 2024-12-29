SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInternalInventoryItems] @From datetime2 = NULL,
                                                  @To datetime2 = NULL,
                                                  @Symbol varchar(100) = NULL,
                                                  @CreatingType varchar(40) = NULL,
                                                  @Status varchar(20) = NULL
AS
    SET NOCOUNT ON;
WITH CTE_PageRecordIds as (SELECT TOP (1) WITH TIES items.[Id],
                                                    items.[Version] as [MaxVersion]
                           FROM [dbo].[InternalInventoryItems] items with (nolock)
                           WHERE (@From is NULL or items.[CreatedAt] >= @From)
                             and (@To is NULL or items.[CreatedAt] <= @To)
                             and (@Symbol is NULL or @Symbol = '' or items.[Symbol] = @Symbol)
                             and (@CreatingType is NULL or @CreatingType = '' or items.[CreatingType] = @CreatingType)
                             and (@Status is NULL or @Status = '' or items.[Status] = @Status)
                           ORDER BY ROW_NUMBER() OVER (PARTITION BY items.[Id] ORDER BY items.[Version] DESC))

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
GO
