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

SELECT I.userId,purchaseGroup,title,upc,description,brand,category,quantity,scandate,imageurl
FROM Items as I INNER JOIN Users as U
ON I.userId = U.UserID

select COUNT(*)
from Items

select *
from Items

select upc
from Items
where ean = 051000059772

--Use this for master list of items. It will NOT truncate the results. (At least with 11 items)
SELECT (select * from Items WHERE userId = 'UID00000001' FOR JSON PATH, ROOT('Items'))
--Use this one \/
SELECT (select * from Items FOR JSON PATH)


INSERT INTO Items(userId,purchaseGroup,ean,title,upc,description,brand,model,weight,category,quantity,scandate,imageurl)
VALUES('UID00000001',
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


GO
ALTER PROCEDURE increaseQuan(@upcInput AS VARCHAR(255)) AS

BEGIN
   UPDATE
      Items 
   set
      quantity = quantity + 1 
   FROM
      Items 
   WHERE
      upc = @upcInput 
END;


EXEC increaseQuan 51000059772;



UPDATE Items set quantity = quantity + 1
where upc = 79400764201


DELETE FROM Items
WHERE ean = 51000197597;

SELECT (select * from Items WHERE userId = 'UID00000001' FOR JSON PATH, ROOT('Items'))
