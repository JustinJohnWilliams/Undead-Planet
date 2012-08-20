USE [UndeadEarth]
GO

/****** Object:  Table [dbo].[UserCounts]    Script Date: 08/22/2010 18:00:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserCounts](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[ZombiesKilled] [bigint] NULL,
	[ZombiePacksDestroyed] [bigint] NULL,
	[Miles] [bigint] NULL,
	[HotZonesDestroyed] [bigint] NULL,
	[PeakAttack] [bigint] NULL,
	[AccumulatedMoney] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


