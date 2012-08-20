create table [dbo].[UserItems]
(
	[UserItemId] uniqueidentifier primary key,
	[UserId] uniqueidentifier,
	[ItemId] uniqueidentifier
)


create table [dbo].Items
(
	[Id] uniqueidentifier primary key,
	[Name] nvarchar(100) not null,
	[Description] nvarchar(255) not null,
	[Price] int not null,
	[Energy] int,
	[Distance] int
)