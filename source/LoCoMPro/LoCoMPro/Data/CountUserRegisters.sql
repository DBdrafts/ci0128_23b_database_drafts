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
