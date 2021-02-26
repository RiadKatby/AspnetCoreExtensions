IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Users'))
BEGIN
	CREATE TABLE Users(
		UserId INT NOT NULL PRIMARY KEY,
		UserName nVarChar(50) NOT NULL,
		Email nVarChar(50) NOT NULL,
		Mobile nVarChar(50) NULL,
		LastLoginTime datetime,
		EmailConfirmed bit not null
	)
END