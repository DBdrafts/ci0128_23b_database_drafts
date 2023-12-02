CREATE FUNCTION dbo.GetStdDevPriceValue (
    @ProductName NVARCHAR(450),
    @StoreName NVARCHAR(450),
    @CantonName NVARCHAR(450),
    @ProvinciaName NVARCHAR(450)
)
RETURNS REAL
AS
BEGIN
    DECLARE @StdDevPriceValue REAL

    DECLARE @MeanPrice REAL
    SELECT @MeanPrice = AVG(Price)
    FROM Register
    WHERE ProductName = @ProductName
      AND StoreName = @StoreName
      AND CantonName = @CantonName
      AND ProvinciaName = @ProvinciaName

    SELECT @StdDevPriceValue = SQRT(AVG(POWER(Price - @MeanPrice, 2)))
    FROM Register
    WHERE ProductName = @ProductName
      AND StoreName = @StoreName
      AND CantonName = @CantonName
      AND ProvinciaName = @ProvinciaName

    RETURN @StdDevPriceValue
END


