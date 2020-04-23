Use LazerScanner

DROP TABLE Users

CREATE TABLE Users (
		ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
		UserID AS 'UID' + RIGHT('00000000' + CAST(ID AS VARCHAR(8)), 8) PERSISTED,
		[email] nvarchar(255),
		[password] nvarchar(255)
		);

INSERT INTO Users(email,password)
VALUES('sample@email.com','BadPassword123')


SELECT * 
FROM Users

