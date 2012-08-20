insert into UserHotZoneProgress (
	Id, UserId, HotZoneId, 
	ZombiesLeft, LastHuntDate, MaxZombies,
	RegenZombieRate, RegenMinuteTicks, LastRegen,
	IsDestroyed)
select	Id = newid(),
		UserId = 'AAAFCB18-3BBD-4842-A1D2-8D19D68EF52E',
		HotZoneId = Id,
		ZombiesLeft = 100,
		LastHuntDate = null,
		MaxZombies = 100,
		RegenZombieRate = 5,
		RegenMinuteTicks = 60,
		LastRegen = '01/03/2010',
		IsDestroyed = 0
from HotZones