SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetLocatorQuoteResponses]
    @From datetime,
    @To datetime,
    @Skip as INT,
    @Take as INT
AS
BEGIN
    SET NOCOUNT ON;

    WITH CTE_PageRecordIds as (SELECT [RecordId]
                               FROM [dbo].[LocatorQuoteResponses] with (nolock)
                               where Time between @From and @To
                               order by [Time] desc

                               OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY)

    SELECT   lqr.[Id]
         ,lqr.[AccountId]
         ,lqr.[Symbol]
         ,lqr.[Status]
         ,lqr.[Time]
         ,lqr.[ErrorMessage]
         ,lqr.[ReqQty]
         ,lqr.[FillQty]
         ,lqr.[Price]
         ,lqr.[DiscountedPrice]
         ,lqr.[Source]
         ,lqr.[Sources]
    from [dbo].[LocatorQuoteResponses] as lqr with (nolock)
    WHERE EXISTS(SELECT 1 FROM CTE_PageRecordIds WHERE CTE_PageRecordIds.[RecordId] = lqr.[RecordId])



END
GO
