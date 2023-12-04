CREATE PROCEDURE UpdateProductName
    @newProductName NVARCHAR(100),
    @oldProductName NVARCHAR(100)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM Product WHERE Name = @newProductName) OR NOT EXISTS (SELECT 1 FROM Product WHERE Name = @oldProductName)
    BEGIN
        -- Parameter not found, print a message and exit the procedure
        PRINT 'New product name not found in products.';
        RETURN;
    END
	IF (@oldProductName = @newProductName)
	BEGIN
		-- Parameters are the same, print a message and exit the procedure
		PRINT 'The name of both parameters is the same.';
        RETURN;
	END

	DECLARE @oldProductBrand NVARCHAR(100);
	DECLARE @oldProductModel NVARCHAR(100);

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
	-- Declare variables
	DECLARE @oldStoreName NVARCHAR(450);
	DECLARE @oldCantonName NVARCHAR(450);
	DECLARE @oldProvinceName NVARCHAR(450);

	-- Create a cursor to iterate through rows with the old product name
	DECLARE cursorSells CURSOR FOR
		SELECT StoreName, CantonName, ProvinceName
		FROM Sells
		WHERE ProductName = @oldProductName;

	-- Open the cursor
	OPEN cursorSells;

	-- Fetch the first row
	FETCH NEXT FROM cursorSells INTO @oldStoreName, @oldCantonName, @oldProvinceName;

	-- Loop through the cursor
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF EXISTS (SELECT 1 FROM SELLS
			WHERE ProductName = @newProductName AND
			StoreName = @oldStoreName AND
			CantonName = @oldCantonName AND
			ProvinceName = @oldProvinceName)
		BEGIN
			-- Delete the current row with the new product name for the same store
			DELETE FROM Sells
			WHERE ProductName = @oldProductName
				AND StoreName = @oldStoreName
				AND CantonName = @oldCantonName
				AND ProvinceName = @oldProvinceName;
		END
		ELSE
		BEGIN
			-- Update the current row in Sells
			UPDATE Sells
			SET ProductName = @newProductName
			WHERE CURRENT OF cursorSells;
		END

		-- Fetch the next row
		FETCH NEXT FROM cursorSells INTO @oldStoreName, @oldCantonName, @oldProvinceName;
	END
	-- Close and deallocate the cursor
	CLOSE cursorSells;
	DEALLOCATE cursorSells;

	-- Declare variables
	DECLARE @oldCategoryName NVARCHAR(450);

	-- Create a cursor to iterate through rows with the old product name
	DECLARE cursorAsociatedWith CURSOR FOR
		SELECT CategoryName
		FROM AsociatedWith
		WHERE ProductName = @oldProductName;

	-- Open the cursor
	OPEN cursorAsociatedWith;

	-- Fetch the first row
	FETCH NEXT FROM cursorAsociatedWith INTO @oldCategoryName;

	-- Loop through the cursor
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF EXISTS (SELECT 1 FROM AsociatedWith
			WHERE ProductName = @newProductName AND
				CategoryName = @oldCategoryName)
		BEGIN
		-- Delete the current row with the new product name for the same store
		DELETE FROM AsociatedWith
		WHERE ProductName = @oldProductName
			AND CategoryName = @oldCategoryName;
		END
		ELSE
		BEGIN
			-- Update the current row in Sells
			UPDATE AsociatedWith
			SET ProductName = @newProductName
			WHERE CURRENT OF cursorAsociatedWith;
		END

		-- Fetch the next row
		FETCH NEXT FROM cursorAsociatedWith INTO @oldCategoryName;
	END
	-- Close and deallocate the cursor
	CLOSE cursorAsociatedWith;
	DEALLOCATE cursorAsociatedWith;

	UPDATE Register
	SET ProductName = @newProductName
	WHERE ProductName = @oldProductName;
	
	-- Remove Product from product table if name matches.
	DELETE Product
	WHERE Name = @oldProductName;
END;