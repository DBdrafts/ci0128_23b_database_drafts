GO
CREATE VIEW SearchRegister AS
SELECT r.SubmitionDate, r.ContributorId, r.ProductName, p.Brand, p.Model,
	r.StoreName, r.Price, c.ProvinciaName, c.CantonName, c.Geolocation
	
FROM Product p INNER JOIN Register r ON p.Name = r.ProductName
			INNER JOIN Canton c ON c.ProvinciaName = r.ProvinciaName AND
								c.CantonName = r.CantonName;
GO;

GO
CREATE FUNCTION CalculateDistance (
	@basePoint geography,
	@otherPoint geography
)
RETURNS FLOAT
AS
BEGIN
	DECLARE @distance FLOAT;
	DECLARE @dummyPoint geography;

	SET @dummyPoint = geography::STPointFromText('POINT(0 0)', 4326);

	IF @basePoint IS NULL OR @otherPoint IS NULL OR @basePoint.STDistance(@dummyPoint) = 0.0
	BEGIN
		RETURN 0.0;
	END
	SET @distance = @basePoint.STDistance(@otherPoint) / 1000.0;

	RETURN @distance;
END
GO;

GO
CREATE PROCEDURE GetSearchResults
    @searchType NVARCHAR(255),
    @searchString NVARCHAR(255),
    @latitude DOUBLE PRECISION  = 0.0,
	@longitude DOUBLE PRECISION  = 0.0
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for possible SQL inyection
    IF PATINDEX('%''%', @searchString) > 0 OR
       PATINDEX('%;%', @searchString) > 0
    BEGIN
        PRINT 'Invalid search string';
        RETURN;
    END;

	DECLARE @basePoint geography;

	SET @basePoint = geography::STPointFromText('POINT(' + CONVERT(VARCHAR(50), @longitude) + ' ' + CONVERT(VARCHAR(50), @latitude) + ')', 4326);

    DECLARE @sql NVARCHAR(MAX);


    -- Start building the dynamic SQL query
    SET @sql = 'SELECT *, dbo.CalculateDistance(@basePoint, Geolocation) AS DISTANCE';

    -- Continue building the query
    SET @sql = @sql + ' FROM SearchRegister WHERE ';

    -- Change the WHERE clause based on the searchType
    IF @searchType = 'Nombre' OR @searchType is NULL
    BEGIN
        SET @sql = @sql + 'LOWER(ProductName) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
    ELSE IF @searchType = 'Tienda'
    BEGIN
        SET @sql = @sql + 'LOWER(StoreName) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
    ELSE IF @searchType = 'Marca'
    BEGIN
        SET @sql = @sql + 'LOWER(Brand) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
	ELSE IF @searchType = 'Modelo'
    BEGIN
        SET @sql = @sql + 'LOWER(Model) LIKE LOWER(''%'' + @searchString + ''%'')';
    END
	SET @sql = @sql + 'ORDER BY DISTANCE ASC';
    -- Execute the dynamic SQL query
    EXEC sp_executesql @sql, N'@searchString NVARCHAR(255), @basePoint geography', @searchString, @basePoint;
END;
GO;