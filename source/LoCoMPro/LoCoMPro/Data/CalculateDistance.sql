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