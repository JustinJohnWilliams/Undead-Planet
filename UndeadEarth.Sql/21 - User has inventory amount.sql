ALTER TABLE dbo.Users ADD
	PossibleItemAmount int NOT NULL CONSTRAINT DF_Users_PossibleItemAmount DEFAULT 5
GO