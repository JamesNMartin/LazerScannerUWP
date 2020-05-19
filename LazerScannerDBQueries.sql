use LazerScanner

DROP TABLE Items


/* ITEM TABLE FOR STORING USER ITEMS */
/* MARK PERISABLE AS BOOL IN THE TABLE */
CREATE TABLE Items (
		[userId] VARCHAR(11),
		[purchaseGroup] nvarchar(255),
		[ean] bigint,
		[title] nvarchar(255),
		[upc] bigint,
		[description] nvarchar(max),
		[brand] nvarchar(255),
		[model] nvarchar(255),
		[weight] nvarchar(255),
		[category] nvarchar(255),
		[quantity] int,
		[scandate] date,
		[imageurl] nvarchar(max)
	);

/* ITEM TABLE FOR STORING ITEM DATA COPIED FROM API */
/* APPLICATION WILL CHECK STORED ITEMS FIRST BEFORE CALLING THE API LOOKUP */
CREATE TABLE StoredItems (
		[ean] bigint,
		[title] nvarchar(255),
		[upc] bigint,
		[description] nvarchar(max),
		[brand] nvarchar(255),
		[model] nvarchar(255),
		[weight] nvarchar(255),
		[category] nvarchar(255),
		[imageurl] nvarchar(max),
		PRIMARY KEY( [ean] )
	);

select *
from StoredItems

SELECT I.userId,purchaseGroup,title,upc,description,brand,category,quantity,scandate,imageurl
FROM Items as I INNER JOIN Users as U
ON I.userId = U.UserID

select COUNT(*)
from Items

select *
from Items
Where upc = 41321241789

select upc
from Items
where ean = 051000059772

--Use this for master list of items. It will NOT truncate the results. (At least with 11 items)
SELECT (select * from Items WHERE userId = 'UID00000004' FOR JSON PATH, ROOT('Items'))
--Use this one \/
SELECT (select * from Items FOR JSON PATH)


INSERT INTO Items(userId,purchaseGroup,ean,title,upc,description,brand,model,weight,category,quantity,scandate,imageurl)
VALUES('UID00000003',
	   '000342400301102003311302',
	   '051000059772',
	   'Campbell''s Healthy Request Cream Of Chicken Soup, 10.75 Oz',
	   '051000059772',
	   'ingredients',
	   'Campbell Soup Company',
	   '200000005977',
	   '13.3 Pounds',
	   'Food, Beverages & Tobacco > Food Items > Soups & Broths',
	   '1',
	   '08/24/2019',
	   'https://target.scene7.com/is/image/Target/GUEST_22949660-e631-4f8d-adbb-85368095658f?wid=1000&hei=1000')

--############################################################################
GO 
ALTER PROCEDURE insertData(@userId VARCHAR(11),
							@purchaseGroup NVARCHAR(255),
							@ean bigint,
							@title NVARCHAR(255),
							@upc bigint,
							@description nvarchar(max),
							@brand nvarchar(255),
							@model nvarchar(255),
							@weight nvarchar(255),
							@category nvarchar(255),
							@quantity int,
							@scandate date,
							@imageurl nvarchar(max)) AS

BEGIN
	INSERT INTO 
			Items
			(userId,
			purchaseGroup,
			ean,
			title,
			upc,
			description,
			brand,
			model,
			weight,
			category,
			quantity,
			scandate,
			imageurl)
	VALUES
			(@userId,
			@purchaseGroup,
			@ean,
			@title,
			@upc,
			@description,
			@brand,
			@model,
			@weight,
			@category,
			@quantity,
			@scandate,
			@imageurl)
	INSERT INTO 
			StoredItems
			(ean,
			title,
			upc,
			description,
			brand,
			model,
			weight,
			category,
			imageurl)
	VALUES
			(@ean,
			@title,
			@upc,
			@description,
			@brand,
			@model,
			@weight,
			@category,
			@imageurl)
END;

EXEC insertData 'UID00000003',
	   '000342400301102003311302',
	   '051000059772',
	   'Campbell''s Healthy Request Cream Of Chicken Soup, 10.75 Oz',
	   '051000059772',
	   'ingredients',
	   'Campbell Soup Company',
	   '200000005977',
	   '13.3 Pounds',
	   'Food, Beverages & Tobacco > Food Items > Soups & Broths',
	   '1',
	   '08/24/2019',
	   'https://target.scene7.com/is/image/Target/GUEST_22949660-e631-4f8d-adbb-85368095658f?wid=1000&hei=1000'
--############################################################################
GO
ALTER PROCEDURE increaseQuan(@upcInput AS VARCHAR(255), @userID as VARCHAR(11)) AS

BEGIN
   UPDATE
      Items 
   set
      quantity = quantity + 1 
   FROM
      Items 
   WHERE
      upc = @upcInput AND userId = @userID
END;


EXEC increaseQuan 51000059772, 'UID00000001';

--############################################################################
GO
ALTER PROCEDURE updateQuan(@upcInput AS VARCHAR(255), @userID as VARCHAR(11), @theQuan as int) AS

BEGIN
	UPDATE
		Items
	SET
		quantity = @theQuan
	FROM
		Items
	WHERE upc = @upcInput AND userId = @userID
END;

EXEC updateQuan 41321241789, 'UID00000001', 2

--############################################################################
GO 
CREATE PROCEDURE deleteItem(@upcInput AS VARCHAR(255), @userID as VARCHAR(11)) AS

BEGIN
	DELETE
	FROM Items
	WHERE upc = @upcInput AND userId = @userID
END;

EXEC deleteItem 41321241789, 'UID00000001'

--############################################################################
UPDATE Items set quantity = quantity + 1
where upc = 79400764201


DELETE FROM Items
WHERE ean = 12000192906;

SELECT (select * from Items WHERE userId = 'UID00000001' FOR JSON PATH, ROOT('Items'))
