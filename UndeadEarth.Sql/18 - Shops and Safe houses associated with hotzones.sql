GO
ALTER TABLE dbo.Stores ADD
	HotZoneId uniqueidentifier NULL
GO
ALTER TABLE dbo.SafeHouses ADD
	HotZoneId uniqueidentifier NULL
GO


UPDATE SafeHouses
SET HotZoneId = '81ABC8B3-E1EB-48F6-9643-3F3C1D064132'
WHERE 1=1 
	AND 3963.191 * 
	ACOS((SIN(PI() * '32.7781' / 180) * SIN(PI() * SafeHouses.latitude / 180)) 
	+ (COS(PI() * '32.7781' /180) * cos(PI() * SafeHouses.latitude / 180) 
		* COS(PI() * SafeHouses.longitude / 180 - PI() * '-96.7954' / 180)) ) <= '100'
		
go

UPDATE Stores
SET HotZoneId = '81ABC8B3-E1EB-48F6-9643-3F3C1D064132'
WHERE 1=1 
	AND 3963.191 * 
	ACOS((SIN(PI() * '32.7781' / 180) * SIN(PI() * Stores.latitude / 180)) 
	+ (COS(PI() * '32.7781' /180) * cos(PI() * Stores.latitude / 180) 
		* COS(PI() * Stores.longitude / 180 - PI() * '-96.7954' / 180)) ) <= '100'