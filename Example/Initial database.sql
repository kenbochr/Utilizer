USE [master]
GO
/****** Object:  Database [Utilizer]    Script Date: 04-06-2017 12:20:53 ******/
CREATE DATABASE [Utilizer]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Utilizer', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\Utilizer.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Utilizer_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\Utilizer_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Utilizer].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Utilizer] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Utilizer] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Utilizer] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Utilizer] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Utilizer] SET ARITHABORT OFF 
GO
ALTER DATABASE [Utilizer] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Utilizer] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Utilizer] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Utilizer] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Utilizer] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Utilizer] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Utilizer] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Utilizer] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Utilizer] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Utilizer] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Utilizer] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Utilizer] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Utilizer] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Utilizer] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Utilizer] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Utilizer] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Utilizer] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Utilizer] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Utilizer] SET  MULTI_USER 
GO
ALTER DATABASE [Utilizer] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Utilizer] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Utilizer] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Utilizer] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
USE [Utilizer]
GO
/****** Object:  Table [dbo].[Activities]    Script Date: 04-06-2017 12:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Activities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoomId] [int] NULL,
	[DateTime] [datetime] NULL,
	[Activity] [bit] NULL,
 CONSTRAINT [PK_Activities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rooms]    Script Date: 04-06-2017 12:20:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rooms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Room] [nvarchar](50) NULL,
 CONSTRAINT [PK_Rooms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Activities] ON 

INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (1, 1, CAST(N'2017-01-01T08:00:00.000' AS DateTime), 1)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (2, 1, CAST(N'2017-01-01T09:00:00.000' AS DateTime), 0)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (3, 1, CAST(N'2017-01-01T11:02:00.000' AS DateTime), 1)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (4, 1, CAST(N'2017-01-01T13:00:00.000' AS DateTime), 0)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (5, 2, CAST(N'2017-01-01T08:00:00.000' AS DateTime), 1)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (6, 2, CAST(N'2017-01-01T09:00:00.000' AS DateTime), 0)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (7, 2, CAST(N'2017-01-01T11:02:00.000' AS DateTime), 1)
INSERT [dbo].[Activities] ([Id], [RoomId], [DateTime], [Activity]) VALUES (8, 2, CAST(N'2017-01-01T13:00:00.000' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[Activities] OFF
SET IDENTITY_INSERT [dbo].[Rooms] ON 

INSERT [dbo].[Rooms] ([Id], [Room]) VALUES (1, N'6119')
INSERT [dbo].[Rooms] ([Id], [Room]) VALUES (2, N'6120')
SET IDENTITY_INSERT [dbo].[Rooms] OFF
ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [FK_Activities_Activities] FOREIGN KEY([Id])
REFERENCES [dbo].[Activities] ([Id])
GO
ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [FK_Activities_Activities]
GO
ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [FK_Activities_Activities1] FOREIGN KEY([Id])
REFERENCES [dbo].[Activities] ([Id])
GO
ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [FK_Activities_Activities1]
GO
ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [FK_Activities_Rooms] FOREIGN KEY([RoomId])
REFERENCES [dbo].[Rooms] ([Id])
GO
ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [FK_Activities_Rooms]
GO
USE [master]
GO
ALTER DATABASE [Utilizer] SET  READ_WRITE 
GO
