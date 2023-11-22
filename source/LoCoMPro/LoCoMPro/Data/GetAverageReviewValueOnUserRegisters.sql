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
