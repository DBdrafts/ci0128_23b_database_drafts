CREATE PROCEDURE GetSearchResults
    @searchType NVARCHAR(255),
    @searchString NVARCHAR(255),
    @basePoint geography = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql NVARCHAR(MAX);
	--DECLARE @distance FLOAT;

    -- Start building the dynamic SQL query
    SET @sql = 'SELECT *, dbo.CalculateDistance(@basePoint, Geolocation) AS DISTANCE';

    -- Continue building the query
    SET @sql = @sql + ' FROM SearchRegister WHERE ';

    -- Change the WHERE clause based on the searchType
    IF @searchType = 'Name' OR @searchType is NULL
    BEGIN
        SET @sql = @sql + 'LOWER(ProductName) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
    ELSE IF @searchType = 'Store'
    BEGIN
        SET @sql = @sql + 'LOWER(StoreName) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
    ELSE IF @searchType = 'Brand'
    BEGIN
        SET @sql = @sql + 'LOWER(Brand) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
	ELSE IF @searchType = 'Model'
    BEGIN
        SET @sql = @sql + 'LOWER(Model) LIKE LOWER(''%'' + @searchString + ''%'')';
    END


    -- Execute the dynamic SQL query
    EXEC sp_executesql @sql, N'@searchString NVARCHAR(255), @basePoint geography', @searchString, @basePoint;
END;

--use [LoCoMProContext-ec360c0e-cf78-4962-821f-b52a2cc4d7a7]; 
--EXEC GetSearchResults 'Name', 'e';