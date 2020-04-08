use ItemDirectory

/*CREATE TABLE Items (
		[id] int not null identity,
		[ean] nvarchar(255),
		[title] nvarchar(255),
		[upc] nvarchar(255),
		[description] TEXT,
		[brand] nvarchar(255),
		[model] nvarchar(255),
		[weight] nvarchar(255),
		[quantity] nvarchar(255),
		[scandate] date,
		PRIMARY KEY( [id] )
	);
*/

DROP TABLE Items


/* TABLE WITH INDEX AS EAN */
CREATE TABLE Items (
		[userId] int,
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
		[imageurl] nvarchar(max),
		PRIMARY KEY( [ean] )
	);


select COUNT(*)
from Items

select *
from Items

select upc
from Items
where ean = 051000059772

--Dont use this one because it truncates the results
--select *
--from Items
--FOR JSON PATH, ROOT('Items')

--Use this for master list of items. It will NOT truncate the results. (At least with 11 items)
SELECT (select * from Items FOR JSON PATH, ROOT('Items'))
SELECT (select * from Items FOR JSON PATH)


INSERT INTO Items(purchaseGroup,ean,title,upc,description,brand,model,weight,category,quantity,scandate,imageurl)
VALUES('000342400301102003311302',
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
WHERE ean = 79400457400;

