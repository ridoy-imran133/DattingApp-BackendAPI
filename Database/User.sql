CREATE TABLE [User](
		"Id" [nvarchar](450) NOT NULL,
		"Username" [nvarchar](450) NOT NULL,
		"PasswordHash" varbinary(max) NOT NULL,
		"PasswordSalt" varbinary(max) NOT NULL,
		"Gender" [nvarchar](16) NULL,
		"DateOfBirth" DATETIME NULL,
		"KnownAs" [nvarchar](16) NULL,
		"CreatedDate" DATETIME NULL,
		"LastActive" DATETIME NULL,
		"Introduction" [nvarchar](450) NULL,
		"LookingFor" [nvarchar](64) NULL,
		"Interests" [nvarchar](128) NULL,
		"City" [nvarchar](64) NULL,
		CONSTRAINT PK_USER PRIMARY KEY ("Id")
		)
		
		// "PasswordSalt" [nvarchar](MAX) NOT NULL,