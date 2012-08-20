-- Sets ZombiePacks HotZoneId to Dallas within a 100 mile radius of Dallas, TX
UPDATE ZombiePacks
SET HotZoneId = '81ABC8B3-E1EB-48F6-9643-3F3C1D064132'
WHERE 1=1 
	AND 3963.191 * 
	ACOS((SIN(PI() * '32.7781' / 180) * SIN(PI() * ZombiePacks.latitude / 180)) 
	+ (COS(PI() * '32.7781' /180) * cos(PI() * ZombiePacks.latitude / 180) 
		* COS(PI() * ZombiePacks.longitude / 180 - PI() * '-96.7954' / 180)) ) <= '100'

-- Sets ZombiePacks HotZoneId to Houston within a 100 mile radius of Houston, TX
UPDATE ZombiePacks
SET HotZoneId = '7270330C-F873-4452-9ACE-D1E6BA1A74F6'
WHERE 1=1 
	AND 3963.191 * 
	ACOS((SIN(PI() * '29.7604' / 180) * SIN(PI() * ZombiePacks.latitude / 180)) 
	+ (COS(PI() * '29.7604' /180) * cos(PI() * ZombiePacks.latitude / 180) 
		* COS(PI() * ZombiePacks.longitude / 180 - PI() * '-95.3697' / 180)) ) <= '100'


delete ZombiePacks from ZombiePacks where HotZoneId != '81ABC8B3-E1EB-48F6-9643-3F3C1D064132' and HotZoneId != '7270330C-F873-4452-9ACE-D1E6BA1A74F6'
delete UserZombiePackProgress from UserZombiePackProgress where ZombiePackId not in(select Id from ZombiePacks)