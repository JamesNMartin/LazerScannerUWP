 Use LazerScanner

DROP TABLE Users

CREATE TABLE Users (
		ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
		UserID AS 'UID' + RIGHT('00000000' + CAST(ID AS VARCHAR(8)), 8) PERSISTED,
		[email] nvarchar(255),
		[password] nvarchar(255)
		);

INSERT INTO Users(email,password)OUTPUT inserted.UserID VALUES('sample@email.com','5amp13Pa5sw0rd') 

SELECT * 
FROM Users

SELECT UserID FROM Users WHERE email = 'jamesnicholasmartin@gmail.com' AND password='password'
SELECT (SELECT UserID FROM Users WHERE email = 'jamesnicholasmartin@gmail.com' AND password='password')

DELETE FROM Users
WHERE UserID = 'UID00000003';