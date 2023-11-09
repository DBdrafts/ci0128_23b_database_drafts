CREATE TRIGGER UpdateUserRoleOnRegister
ON Register
AFTER INSERT
AS
BEGIN
	DECLARE @UserId NVARCHAR(450)
	DECLARE @RegisterCount INT
	DECLARE @AverageReviewValue REAL

	SELECT @UserId = ContributorId
	FROM inserted

	SELECT @RegisterCount = dbo.CountUserRegisters(@UserId)

	SELECT @AverageReviewValue = dbo.GetAverageReviewValueOnUserRegisters(@UserId)

	IF @RegisterCount > 10 AND @AverageReviewValue > 4.7
	BEGIN
    	UPDATE [User]
   	 SET Role = 'Moderator'
   	 WHERE Id = @UserId
	END
END
