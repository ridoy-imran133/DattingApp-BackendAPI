CREATE TABLE [Photo](
	"Id" [nvarchar](450) NOT NULL,
	"UserId" [nvarchar](450) NULL,
	"Url" [nvarchar](450) NULL,
	"Description" [nvarchar](450) NULL,
	"DateAdded" DATETIME NULL,
	"IsMain" BIT NOT NULL DEFAULT 0,
	CONSTRAINT PK_PHOTO PRIMARY KEY ("Id"),
	CONSTRAINT UK_PHOTO UNIQUE("UserId")
)