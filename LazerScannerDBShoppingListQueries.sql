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