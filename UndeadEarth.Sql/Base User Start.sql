delete from UserZombiePackProgress
delete from UserCounts
update Users
set [Level] = 1,
	BaseLineEnergy = 100,
	BaseLineAttackPower = 1,
	PossibleItemAmount = 5,
	Money = 0,
	LastEnergy = null,
	CurrentBaseAttack = 1,
	CurrentBaseEnergy = 100
	
delete UserItems
	
insert into UserItems(ItemId, UserId, UserItemId)
select Id,
	   'AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E',
	   NEWID()
from Items
where Name = 'Tent'


insert into UserItems(ItemId, UserId, UserItemId)
select Id,
	   'AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E',
	   NEWID()
from Items
where Name = 'Tent'


insert into UserItems(ItemId, UserId, UserItemId)
select Id,
	   'AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E',
	   NEWID()
from Items
where Name = 'Tent'


insert into UserItems(ItemId, UserId, UserItemId)
select Id,
	   'AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E',
	   NEWID()
from Items
where Name = 'Tent'


insert into UserItems(ItemId, UserId, UserItemId)
select Id,
	   'AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E',
	   NEWID()
from Items
where Name = 'Tent'