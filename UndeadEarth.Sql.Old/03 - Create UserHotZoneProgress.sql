CREATE TABLE [dbo].[UserHotZoneProgress](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[HotZoneId] [uniqueidentifier] NOT NULL,
	[ZombiesLeft] [int] NOT NULL,
	[LastHuntDate] [datetime] NULL,
	[MaxZombies] [int] NOT NULL,
	[RegenZombieRate] [int] NOT NULL,
	[RegenMinuteTicks] [int] NOT NULL,
	[LastRegen] [datetime] NOT NULL,
	[IsDestroyed] [bit] NOT NULL,
 CONSTRAINT [PK_UserHotZoneProgress] PRIMARY KEY CLUSTERED 
 (
	[Id] ASC
 )
 )

GO

ALTER TABLE [dbo].[UserHotZoneProgress] ADD  CONSTRAINT [DF_UserHotZoneProgress_Id]  DEFAULT (newid()) FOR [Id]
GO