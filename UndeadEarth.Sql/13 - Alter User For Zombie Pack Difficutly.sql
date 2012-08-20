/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Users ADD
	LastVisitedHotZoneId uniqueidentifier NULL,
	BaseLineAttackPower int NOT NULL CONSTRAINT DF_Users_MaxAttackPower DEFAULT 1,
	BaseLineEnergy int NOT NULL CONSTRAINT DF_Users_MaxPotentialEnergy DEFAULT 100
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT