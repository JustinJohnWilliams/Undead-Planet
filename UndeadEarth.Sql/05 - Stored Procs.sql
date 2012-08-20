		
CREATE PROCEDURE [dbo].[sp_GetZombiePacksWithinRadius]
(
	@Latitude		decimal(8, 4)
	, @Longitude	decimal(8, 4)
	, @Radius		float
)
AS
SELECT * FROM ZombiePacks where ZombiePacks.Latitude = @Latitude AND ZombiePacks.Longitude = @Longitude
UNION
SELECT * FROM ZombiePacks 
	WHERE 1=1
	AND (ZombiePacks.Latitude != @Latitude AND ZombiePacks.Longitude != @Longitude)
	AND 3963.191 * 
	ACOS((SIN(PI() * @Latitude / 180) * SIN(PI() * ZombiePacks.latitude / 180)) 
	+ (COS(PI() * @Latitude /180) * cos(PI() * ZombiePacks.latitude / 180) 
		* COS(PI() * ZombiePacks.longitude / 180 - PI() * @Longitude / 180)) ) <= @Radius