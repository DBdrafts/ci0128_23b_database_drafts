CREATE FUNCTION dbo.GetAveragePriceValue (
    @ProductName NVARCHAR(450),
    @StoreName NVARCHAR(450),
    @CantonName NVARCHAR(450),
    @ProvinciaName NVARCHAR(450)
)
RETURNS REAL
AS
BEGIN
    DECLARE @AveragePriceValue REAL

    SELECT @AveragePriceValue = AVG(Price)
    FROM Register
    WHERE ProductName = @ProductName
      AND StoreName = @StoreName
      AND CantonName = @CantonName
      AND ProvinciaName = @ProvinciaName

    RETURN @AveragePriceValue
END
