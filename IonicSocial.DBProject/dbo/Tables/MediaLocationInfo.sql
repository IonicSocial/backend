CREATE TABLE [dbo].[MediaLocationInfo] (
    [MediaLocationInfo] BIGINT        IDENTITY (1, 1) NOT NULL,
    [MediaID]           BIT           NOT NULL,
    [Lat]               NVARCHAR (50) NULL,
    [Long]              NVARCHAR (50) NULL,
    [Address1]          NVARCHAR (50) NULL,
    [Address2]          NVARCHAR (50) NULL,
    [ZIP]               NVARCHAR (50) NULL,
    [InsertedBy]        INT           NULL,
    [InsertedDate]      DATETIME      NULL,
    [IsActive]          BIT           CONSTRAINT [DF_MediaLocationInfo_IsActive] DEFAULT ((1)) NOT NULL,
    [IsDeleted]         BIT           CONSTRAINT [DF_MediaLocationInfo_IsDeleted] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_MediaLocationInfo] PRIMARY KEY CLUSTERED ([MediaLocationInfo] ASC)
);

