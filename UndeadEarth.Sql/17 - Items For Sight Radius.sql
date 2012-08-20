insert into Items(Id, Name, Description, Price, Attack, Distance, Energy, IsOneTimeUse)
values(NEWID(), 'Sonar Pulse', 'Temporarily increases sight radius by 1 mile.', 100, 0, 1, 0, 1)

insert into Items(Id, Name, Description, Price, Attack, Distance, Energy, IsOneTimeUse)
values(NEWID(), 'Binoculars', 'Permanently increases sight radius by 1 mile.', 500, 0, 5, 0, 0)

update Items set IsOneTimeUse = 1 where Name = 'Tent'

/*
update Users	
	set LastEnergy = null, 
	LastEnergyDate = null, 
	LastSightRadius = null, 
	LastSightRadiusDate = null, 
	Money = 5000

delete UserItems	
delete SafeHouseItems
delete UserZombiePackProgress
*/