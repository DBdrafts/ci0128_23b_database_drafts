use [LoCoMProContext-ec360c0e-cf78-4962-821f-b52a2cc4d7a7];
-- Funciones y Triggers para el manejo de usuarios
GO
CREATE FUNCTION dbo.CountUserRegisters
(
    @UserId NVARCHAR(450)
)
RETURNS INT
AS
BEGIN
    DECLARE @RegisterCount INT

    SELECT @RegisterCount = COUNT(*) 
    FROM Register
    WHERE ContributorId = @UserId

    RETURN @RegisterCount
END
GO;

GO
CREATE FUNCTION dbo.GetAverageReviewValueOnUserRegisters (
    @UserId NVARCHAR(450)
)
RETURNS REAL
AS
BEGIN
    DECLARE @AvgReviewOnRegisters REAL

    SELECT @AvgReviewOnRegisters = AVG(ReviewValue)
    FROM Review
    WHERE ContributorId = @UserId

    RETURN @AvgReviewOnRegisters
END

GO;
-- Funciones y procedimientos para la página de Producto.
GO
CREATE FUNCTION dbo.GetAverageReviewValue (
    @ContributorId NVARCHAR(450),
    @ProductName NVARCHAR(450),
    @StoreName NVARCHAR(450),
    @SubmitionDate DATETIME2
)
RETURNS REAL
AS
BEGIN
    DECLARE @AverageReviewValue REAL

    SELECT @AverageReviewValue = AVG(ReviewValue)
    FROM Review
    WHERE ContributorId = @ContributorId
      AND ProductName = @ProductName
      AND StoreName = @StoreName
      AND SubmitionDate = @SubmitionDate

    RETURN @AverageReviewValue
END
GO;
-- Funciones y procedimientos para la página de Búsqueda.
GO
CREATE VIEW SearchRegister AS
SELECT r.SubmitionDate, r.ContributorId, r.ProductName, p.Brand, p.Model,
	r.StoreName, r.Price, r.NumCorrections, r.Comment, c.ProvinciaName,
	c.CantonName, c.Geolocation
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

	DECLARE @basePoint geography;
	PRINT 'DECLARED';
	SET @basePoint = geography::STPointFromText('POINT(' + CONVERT(VARCHAR(50), @longitude) + ' ' + CONVERT(VARCHAR(50), @latitude) + ')', 4326);
	PRINT 'SETTED';
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