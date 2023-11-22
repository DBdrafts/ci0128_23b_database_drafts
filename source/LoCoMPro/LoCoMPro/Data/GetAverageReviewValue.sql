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
