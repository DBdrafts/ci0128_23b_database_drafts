CREATE FUNCTION dbo.GetMaxPriceValue (
    @ProductName NVARCHAR(450),
    @StoreName NVARCHAR(450),
    @CantonName NVARCHAR(450),
    @ProvinciaName NVARCHAR(450)
)
RETURNS REAL
AS
BEGIN
    DECLARE @MaxPriceValue REAL

    SELECT @MaxPriceValue = MAX(Price)
    FROM Register
    WHERE ProductName = @ProductName
      AND StoreName = @StoreName
      AND CantonName = @CantonName
      AND ProvinciaName = @ProvinciaName

    RETURN @MaxPriceValue
END
