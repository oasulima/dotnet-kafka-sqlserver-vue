SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetLocatesReportData](
    @Skip as INT,
    @Take as INT,
    @From as datetime = NULL,
    @To as datetime = NULL,
    @OrderBy as nvarchar(100),
    @Symbol as nvarchar(100) = NULL,
    @Status as TINYINT = NULL,
    @AccountId as nvarchar(100) = NULL,
    @ProviderId as nvarchar(100) = NULL)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OrderByLower nvarchar(100) = LOWER(@OrderBy);
    WITH CTE_TotalCount as (select Count(*) as TotalCount
                            FROM [dbo].[LocatorQuoteResponses] with (nolock)
                            WHERE [IsReportableStatus] = 1
                              AND (@From is NULL OR [Time] >= @From)
                              AND (@To is NULL OR [Time] < DATEADD(minute, 1, @To))
                              AND (@Symbol is NULL OR @Symbol = '' OR [Symbol] = @Symbol)
                              AND (@Status is NULL OR [Status] = @Status)
                              AND (@AccountId is NULL OR @AccountId = '' OR [AccountId] = @AccountId)
                              AND (@ProviderId is NULL OR @ProviderId = '' OR exists(SELECT * FROM OPENJSON(Sources) Where Source = @ProviderId))),
         CTE_PageRecordIds as (SELECT [RecordId]
                               FROM [dbo].[LocatorQuoteResponses] with (nolock)
                               WHERE [IsReportableStatus] = 1
                                 AND (@From is NULL OR [Time] >= @From)
                                 AND (@To is NULL OR [Time] < DATEADD(minute, 1, @To))
                                 AND (@Symbol is NULL OR @Symbol = '' OR [Symbol] = @Symbol)
                                 AND (@Status is NULL OR [Status] = @Status)
                                 AND (@AccountId is NULL OR @AccountId = '' OR [AccountId] = @AccountId)
                                 AND (@ProviderId is NULL OR @ProviderId = '' OR exists(SELECT * FROM OPENJSON(Sources) Where Source = @ProviderId))
                               ORDER BY CASE WHEN @OrderByLower = 'accountid asc' THEN [AccountId] END ASC,
                                        CASE WHEN @OrderByLower = 'time asc' THEN [Time] END ASC,
                                        CASE WHEN @OrderByLower = 'symbol asc' THEN [Symbol] END ASC,
                                        CASE WHEN @OrderByLower = 'reqqty asc' THEN [ReqQty] END ASC,
                                        CASE WHEN @OrderByLower = 'fillqty asc' THEN [FillQty] END ASC,
                                        CASE WHEN @OrderByLower = 'price asc' THEN [Price] END ASC,
                                        CASE WHEN @OrderByLower = 'status asc' THEN [Status] END ASC,
                                        CASE WHEN @OrderByLower = 'discountedprice asc' THEN [DiscountedPrice] END ASC,
                                        CASE WHEN @OrderByLower = 'fee asc' THEN [Fee] END ASC,
                                        CASE WHEN @OrderByLower = 'discountedfee asc' THEN [DiscountedFee] END ASC,
                                        CASE WHEN @OrderByLower = 'profit asc' THEN [Profit] END ASC,
                                        CASE WHEN @OrderByLower = 'accountid desc' THEN [AccountId] END DESC,
                                        CASE WHEN @OrderByLower = 'time desc' THEN [Time] END DESC,
                                        CASE WHEN @OrderByLower = 'symbol desc' THEN [Symbol] END DESC,
                                        CASE WHEN @OrderByLower = 'reqqty desc' THEN [ReqQty] END DESC,
                                        CASE WHEN @OrderByLower = 'fillqty desc' THEN [FillQty] END DESC,
                                        CASE WHEN @OrderByLower = 'price desc' THEN [Price] END DESC,
                                        CASE WHEN @OrderByLower = 'status desc' THEN [Status] END DESC,
                                        CASE WHEN @OrderByLower = 'discountedprice desc' THEN [DiscountedPrice] END DESC,
                                        CASE WHEN @OrderByLower = 'fee desc' THEN [Fee] END DESC,
                                        CASE WHEN @OrderByLower = 'discountedfee desc' THEN [DiscountedFee] END DESC,
                                        CASE WHEN @OrderByLower = 'profit desc' THEN [Profit] END DESC

                               OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY)

    Select lqr.[Id]
         , lqr.[AccountId]
         , lqr.[Symbol]
         , lqr.[Status]
         , lqr.[Time]
         , lqr.[ReqQty]
         , lqr.[FillQty]
         , lqr.[Price]
         , lqr.[DiscountedPrice]
         , lqr.[Fee]
         , lqr.[DiscountedFee]
         , lqr.[Profit]
         , lqr.[Source]
         , lqr.[Sources]
         , lqr.[ErrorMessage]
         , TotalCount
    from [dbo].[LocatorQuoteResponses] as lqr with (nolock),
         CTE_TotalCount
    WHERE EXISTS(SELECT 1 FROM CTE_PageRecordIds WHERE CTE_PageRecordIds.[RecordId] = lqr.[RecordId])
    ORDER BY CASE WHEN @OrderByLower = 'accountid asc' THEN [AccountId] END ASC,
             CASE WHEN @OrderByLower = 'time asc' THEN [Time] END ASC,
             CASE WHEN @OrderByLower = 'symbol asc' THEN [Symbol] END ASC,
             CASE WHEN @OrderByLower = 'reqqty asc' THEN [ReqQty] END ASC,
             CASE WHEN @OrderByLower = 'fillqty asc' THEN [FillQty] END ASC,
             CASE WHEN @OrderByLower = 'price asc' THEN [Price] END ASC,
             CASE WHEN @OrderByLower = 'status asc' THEN [Status] END ASC,
             CASE WHEN @OrderByLower = 'discountedprice asc' THEN [DiscountedPrice] END ASC,
             CASE WHEN @OrderByLower = 'fee asc' THEN [Fee] END ASC,
             CASE WHEN @OrderByLower = 'discountedfee asc' THEN [DiscountedFee] END ASC,
             CASE WHEN @OrderByLower = 'profit asc' THEN [Profit] END ASC,
             CASE WHEN @OrderByLower = 'accountid desc' THEN [AccountId] END DESC,
             CASE WHEN @OrderByLower = 'time desc' THEN [Time] END DESC,
             CASE WHEN @OrderByLower = 'symbol desc' THEN [Symbol] END DESC,
             CASE WHEN @OrderByLower = 'reqqty desc' THEN [ReqQty] END DESC,
             CASE WHEN @OrderByLower = 'fillqty desc' THEN [FillQty] END DESC,
             CASE WHEN @OrderByLower = 'price desc' THEN [Price] END DESC,
             CASE WHEN @OrderByLower = 'status desc' THEN [Status] END DESC,
             CASE WHEN @OrderByLower = 'discountedprice desc' THEN [DiscountedPrice] END DESC,
             CASE WHEN @OrderByLower = 'fee desc' THEN [Fee] END DESC,
             CASE WHEN @OrderByLower = 'discountedfee desc' THEN [DiscountedFee] END DESC,
             CASE WHEN @OrderByLower = 'profit desc' THEN [Profit] END DESC

    OPTION (RECOMPILE)
END
GO
