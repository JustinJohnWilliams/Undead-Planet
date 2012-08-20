ALTER TABLE dbo.Items ADD
	Attack int NULL
	
GO	
insert into Items(Id, Name, Price, Description, Distance, Energy, Attack)
values(NEWID(), 'Baseball Bat', 500, 'Increases your attack power by 5.', null, null, 5)