SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetQuoteSymbolQuantities]
    @From datetime2,
    @To datetime2,
    @ExcludeProvidersJson varchar(max) = NULL,
    @IncludeProvidersJson varchar(max) = NULL,
    @StatusesJson varchar(max) = NULL,
    @ExcludeAccountIdsJson varchar(max) = NULL,
    @IncludeAccountIdsJson varchar(max) = NULL
AS
SELECT qResp.[Symbol], SUM(sources.[Qty]) as Quantity
FROM [dbo].[LocatorQuoteResponses] qResp
         CROSS APPLY (
    SELECT [Provider], [Qty] FROM OPENJSON(qResp.[Sources])
                                         WITH (
                                             [Provider] varchar(100),
                                             [Qty] int
                                             )
) sources
WHERE qResp.[Time] between @From AND @To
  and (@ExcludeProvidersJson is NULL or sources.[Provider] not in (SELECT [value] FROM OPENJSON(@ExcludeProvidersJson)))
  and (@IncludeProvidersJson is NULL or sources.[Provider] in (SELECT [value] FROM OPENJSON(@IncludeProvidersJson)))
  and (@StatusesJson is NULL or qResp.[Status] in (SELECT [value] FROM OPENJSON(@StatusesJson)))
  and (@ExcludeAccountIdsJson is NULL or qResp.[AccountId] not in (SELECT [value] FROM OPENJSON(@ExcludeAccountIdsJson)))
  and (@IncludeAccountIdsJson is NULL or qResp.[AccountId] in (SELECT [value] FROM OPENJSON(@IncludeAccountIdsJson)))
GROUP BY qResp.[Symbol]
GO
