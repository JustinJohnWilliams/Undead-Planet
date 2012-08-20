CREATE TABLE [dbo].[UserZombiePackProgress](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ZombiePackId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[UserZombiePackProgress] ADD  CONSTRAINT [DF_UserZombiePackProgress_Id]  DEFAULT (newid()) FOR [Id]
GO