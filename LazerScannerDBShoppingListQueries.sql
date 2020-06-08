Use LazerScanner

DROP TABLE ShoppingList

CREATE TABLE ShoppingList (
		[userId] VARCHAR(11),
		[ean] bigint,
		[title] nvarchar(255),
		[upc] bigint,
		[brand] nvarchar(255),
		[model] nvarchar(255),
		[category] nvarchar(255),
		[imageurl] nvarchar(max)
	);

SELECT *
FROM ShoppingList

SELECT (SELECT COUNT(*) FROM ShoppingList WHERE userId = 'UID00000001')

SELECT (select * from ShoppingList WHERE userId = 'UID00000001' FOR JSON PATH, ROOT('Items'))

--############################################################################
GO 
ALTER PROCEDURE sendToShoppingList(@userId VARCHAR(11),
									@ean bigint,
									@title NVARCHAR(255),
									@upc bigint,
									@brand NVARCHAR(255),
									@model NVARCHAR(255),
									@category NVARCHAR(255),
									@imageurl NVARCHAR(max)) AS
BEGIN
	INSERT INTO ShoppingList (userId,
							  ean,
							  title,
							  upc,
							  brand,
							  model,
							  category,
							  imageurl)
	VALUES (@userId,
			@ean,
			@title,
			@upc,
			@brand,
			@model,
			@category,
			@imageurl)
	DELETE
	FROM Items
	WHERE upc = @upc AND userId = @userID
END;

--############################################################################
GO
CREATE PROCEDURE moveFromShoppingListToInventoryc (@userId @ean bigint) AS BEGIN
		INSERT INTO Items (userId, ean, title, upc, description, brand, model, weight,category,quantity,scandate,imageurl)
		VALUES()
		SELECT ean,title,upc,description,brand,model,weight,category,imageurl
		FROM StoredItems
		WHERE ean = @ean
END;



--############################################################################
