SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInternalInventorySymbolQuantities]
    @From datetime2,
    @To datetime2,
    @ExcludeSourcesJson varchar(max) = NULL,
    @IncludeSourcesJson varchar(max) = NULL,
	@StatusesJson varchar(max) = NULL,
	@CreatingTypesJson varchar(max) = NULL
AS
SELECT items.[Symbol], SUM(items.[Quantity] + items.[SoldQuantity]) as Quantity
FROM [dbo].[InternalInventoryItems] items
WHERE items.[CreatedAt] between @From AND @To
    and (@ExcludeSourcesJson is NULL or items.[Source] not in (SELECT [value] FROM OPENJSON(@ExcludeSourcesJson)))
    and (@IncludeSourcesJson is NULL or items.[Source] in (SELECT [value] FROM OPENJSON(@IncludeSourcesJson)))
    and (@StatusesJson is NULL or items.[Status] in (SELECT [value] FROM OPENJSON(@StatusesJson)))
	and (@CreatingTypesJson is NULL or items.[CreatingType] in (SELECT [value] FROM OPENJSON(@CreatingTypesJson)))
    and items.[Version]=(SELECT MAX(items2.[Version]) FROM [dbo].[InternalInventoryItems] items2 WHERE items.[Id]=items2.[Id])
GROUP BY items.[Symbol]
GO
