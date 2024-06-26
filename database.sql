USE [stsmeepup]
GO
/****** Object:  Table [dbo].[admins]    Script Date: 21.06.2024 12:13:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[admins](
	[id_admin] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](120) NOT NULL,
	[chatid] [varchar](120) NULL,
	[chatStatus] [varchar](120) NULL,
 CONSTRAINT [PK__admins__89472E959925FAE5] PRIMARY KEY CLUSTERED 
(
	[id_admin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[chats]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[chats](
	[id_chat] [int] IDENTITY(1,1) NOT NULL,
	[nameChat] [varchar](120) NOT NULL,
	[chatId] [varchar](120) NULL,
 CONSTRAINT [PK__chats__68D484D1AA9FBDE4] PRIMARY KEY CLUSTERED 
(
	[id_chat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[connectChatAdmin]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[connectChatAdmin](
	[id_connect] [int] IDENTITY(1,1) NOT NULL,
	[id_chat] [int] NOT NULL,
	[id_admin] [int] NULL,
 CONSTRAINT [PK__connectC__9008C3B27748AC13] PRIMARY KEY CLUSTERED 
(
	[id_connect] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[connectUsersFinishedMeetings]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[connectUsersFinishedMeetings](
	[id_connect] [int] IDENTITY(1,1) NOT NULL,
	[id_finishedMeeting] [int] NOT NULL,
	[id_user] [int] NOT NULL,
 CONSTRAINT [PK__connectU__9008C3B20CF0B104] PRIMARY KEY CLUSTERED 
(
	[id_connect] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[finishedMeetings]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[finishedMeetings](
	[id_finishedMeeting] [int] IDENTITY(1,1) NOT NULL,
	[id_meeting] [int] NOT NULL,
	[duration] [int] NOT NULL,
 CONSTRAINT [PK__finished__8540B7DDB8656E6A] PRIMARY KEY CLUSTERED 
(
	[id_finishedMeeting] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[meetings]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[meetings](
	[id_meeting] [int] IDENTITY(1,1) NOT NULL,
	[id_chat] [int] NOT NULL,
	[titleMeeting] [varchar](120) NOT NULL,
	[descriptionMeeting] [text] NOT NULL,
	[dateCreateMeeting] [datetime] NOT NULL,
	[notificationMeeting] [int] NOT NULL,
	[dateEventMeeting] [datetime] NOT NULL,
	[statusMeeting] [int] NOT NULL,
 CONSTRAINT [PK__meetings__00894421C518BF75] PRIMARY KEY CLUSTERED 
(
	[id_meeting] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[typeStatusMeeting]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[typeStatusMeeting](
	[id_status] [int] NOT NULL,
	[nameStatus] [varchar](120) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[users]    Script Date: 21.06.2024 12:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[users](
	[id_user] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](120) NOT NULL,
	[userId] [varchar](120) NOT NULL,
 CONSTRAINT [PK__users__D2D14637A46B94A6] PRIMARY KEY CLUSTERED 
(
	[id_user] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[connectChatAdmin]  WITH CHECK ADD  CONSTRAINT [FK__connectCh__id_ad__173876EA] FOREIGN KEY([id_admin])
REFERENCES [dbo].[admins] ([id_admin])
GO
ALTER TABLE [dbo].[connectChatAdmin] CHECK CONSTRAINT [FK__connectCh__id_ad__173876EA]
GO
ALTER TABLE [dbo].[connectChatAdmin]  WITH CHECK ADD  CONSTRAINT [FK__connectCh__id_ch__164452B1] FOREIGN KEY([id_chat])
REFERENCES [dbo].[chats] ([id_chat])
GO
ALTER TABLE [dbo].[connectChatAdmin] CHECK CONSTRAINT [FK__connectCh__id_ch__164452B1]
GO
ALTER TABLE [dbo].[connectUsersFinishedMeetings]  WITH CHECK ADD  CONSTRAINT [FK__connectUs__id_fi__2DE6D218] FOREIGN KEY([id_finishedMeeting])
REFERENCES [dbo].[finishedMeetings] ([id_finishedMeeting])
GO
ALTER TABLE [dbo].[connectUsersFinishedMeetings] CHECK CONSTRAINT [FK__connectUs__id_fi__2DE6D218]
GO
ALTER TABLE [dbo].[connectUsersFinishedMeetings]  WITH CHECK ADD  CONSTRAINT [FK__connectUs__id_us__2EDAF651] FOREIGN KEY([id_user])
REFERENCES [dbo].[users] ([id_user])
GO
ALTER TABLE [dbo].[connectUsersFinishedMeetings] CHECK CONSTRAINT [FK__connectUs__id_us__2EDAF651]
GO
ALTER TABLE [dbo].[finishedMeetings]  WITH CHECK ADD  CONSTRAINT [FK__finishedM__id_me__2B0A656D] FOREIGN KEY([id_meeting])
REFERENCES [dbo].[meetings] ([id_meeting])
GO
ALTER TABLE [dbo].[finishedMeetings] CHECK CONSTRAINT [FK__finishedM__id_me__2B0A656D]
GO
ALTER TABLE [dbo].[meetings]  WITH CHECK ADD  CONSTRAINT [FK__meetings__id_cha__1BFD2C07] FOREIGN KEY([id_chat])
REFERENCES [dbo].[chats] ([id_chat])
GO
ALTER TABLE [dbo].[meetings] CHECK CONSTRAINT [FK__meetings__id_cha__1BFD2C07]
GO
ALTER TABLE [dbo].[meetings]  WITH CHECK ADD  CONSTRAINT [FK__meetings__status__1CF15040] FOREIGN KEY([statusMeeting])
REFERENCES [dbo].[typeStatusMeeting] ([id_status])
GO
ALTER TABLE [dbo].[meetings] CHECK CONSTRAINT [FK__meetings__status__1CF15040]
GO
