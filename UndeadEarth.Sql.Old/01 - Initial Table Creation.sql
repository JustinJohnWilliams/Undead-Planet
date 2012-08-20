USE [UndeadEarth]
GO

CREATE TABLE [dbo].[HotZones](
	[Id] [uniqueidentifier] NOT NULL,
	[Latitude] [decimal](8, 4) NOT NULL,
	[Longitude] [decimal](8, 4) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_HotZones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

USE [UndeadEarth]
GO

CREATE TABLE [dbo].[InfoNodes](
	[Id] [uniqueidentifier] NOT NULL,
	[Latitude] [decimal](8, 4) NOT NULL,
	[Longitude] [decimal](8, 4) NOT NULL,
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
	[Latitude] [decimal](8, 4) NOT NULL,
	[Longitude] [decimal](8, 4) NOT NULL,
	[TempLatitude] [decimal](8, 4) NULL,
	[TempLongitude] [decimal](8, 4) NULL,
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

