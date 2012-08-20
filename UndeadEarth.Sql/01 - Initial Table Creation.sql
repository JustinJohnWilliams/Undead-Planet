USE [UndeadEarth]
GO

CREATE TABLE [dbo].[ZombiePacks](
	[Id] [uniqueidentifier] NOT NULL,
	[Latitude] [money] NOT NULL,
	[Longitude] [money] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[HotZoneId] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_HotZones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

USE [UndeadEarth]
GO

CREATE TABLE [dbo].[HotZones](
	[Id] [uniqueidentifier] NOT NULL,
	[Latitude] [money] NOT NULL,
	[Longitude] [money] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_InfoNodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

USE [UndeadEarth]
GO

CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[ZoneId] [uniqueidentifier] NOT NULL,
	[LocationId] [uniqueidentifier] NOT NULL,
	[Latitude] [money] NOT NULL,
	[Longitude] [money] NOT NULL,
	[TempLatitude] [money] NULL,
	[TempLongitude] [money] NULL,
	[NextLocationId] [uniqueidentifier] NULL,
	[NextLatitude] [decimal](8, 4) NULL,
	[NextLongitude] [decimal](8, 4) NULL,
	[MoveStartTime] [datetime] NULL,
	[MoveEndTime] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_TempLatitiude]  DEFAULT ((0)) FOR [TempLatitude]
GO

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_TempLongitude]  DEFAULT ((0)) FOR [TempLongitude]
GO

USE [UndeadEarth]
GO

/****** Object:  Table [dbo].[Stores]    Script Date: 07/09/2010 20:39:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Stores](
	[Id] [uniqueidentifier] NOT NULL,
	[Latitude] [money] NOT NULL,
	[Longitude] [money] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO