create table SafeHouses(
	[Id] uniqueidentifier primary key NOT NULL,
	[Latitude] [money] NOT NULL,
	[Longitude] [money] NOT NULL,
)

insert into SafeHouses(Id, Latitude, Longitude)
select Id = NEWID(), Latitude = H.Latitude, Longitude = H.Longitude from HotZones as H

update SafeHouses
set Latitude = Latitude - .02, Longitude = Longitude - .02

create table SafeHouseItems(
	[Id] uniqueidentifier primary key NOT NULL,
	[SafeHouseId] uniqueidentifier NOT NULL,
	[UserId] uniqueidentifier NOT NULL,
	[ItemId] uniqueidentifier NOT NULL
)

