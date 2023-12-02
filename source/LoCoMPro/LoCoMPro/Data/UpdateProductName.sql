CREATE PROCEDURE UpdateProductName
    @newProductName NVARCHAR(100),
    @oldProductName NVARCHAR(100)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM Product WHERE Name = @oldProductName)
    BEGIN
        -- Parameter not found, print a message and exit the procedure
        PRINT 'New product name not found in products.';
        RETURN;
    END
	IF (@oldProductName = @newProductName)
	BEGIN
		-- Parameters are the same, print a message and exit the procedure
		PRINT 'The name of both parameters is the saem.';
        RETURN;
	END

	DECLARE @oldProductBrand NVARCHAR(MAX);
	DECLARE @oldProductModel NVARCHAR(MAX);

	SET @oldProductBrand = (SELECT Brand FROM Product WHERE Name = @oldProductName);
	SET @oldProductModel = (SELECT Model FROM Product WHERE Name = @oldProductName);

	-- Update Product table if data is new
	UPDATE Product
	SET Brand = @oldProductBrand
	WHERE Brand is NULL AND Name = @newProductName;

	UPDATE Product
	SET Model = @oldProductModel
	WHERE Model is NULL AND Name = @newProductName;

	-- Update Tables with foreign keys.
    UPDATE Sells
	SET ProductName = @newProductName
	WHERE ProductName = @oldProductName;

	UPDATE AsociatedWith
	SET ProductName = @newProductName
	WHERE ProductName = @oldProductName;
	
	UPDATE Register
	SET ProductName = @newProductName
	WHERE ProductName = @oldProductName;
	
	-- Remove Product from product table if name matches.
	DELETE Product
	WHERE Name = @oldProductName;
END;