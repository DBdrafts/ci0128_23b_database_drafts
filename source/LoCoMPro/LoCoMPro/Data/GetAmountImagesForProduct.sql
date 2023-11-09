CREATE FUNCTION dbo.GetAmountImagesForProduct (
    @ContributorId NVARCHAR(450),
    @ProductName NVARCHAR(450),
    @StoreName NVARCHAR(450),
    @SubmitionDate DATETIME2
)
	RETURNS INT
AS
BEGIN
    DECLARE @AmountOfImages INT

    SELECT @AmountOfImages = COUNT(*)
    FROM Image
    WHERE ContributorId = @ContributorId
      AND ProductName = @ProductName
      AND StoreName = @StoreName
      AND SubmitionDate = @SubmitionDate

      RETURN @AmountOfImages
END
