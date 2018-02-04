CREATE TABLE [dbo].[UserMedia] (
    [MediaID]          BIGINT          IDENTITY (1000, 21) NOT NULL,
    [UserID]           NVARCHAR (128)  NOT NULL,
    [Title]            NVARCHAR (100)  NOT NULL,
    [MediaType]        INT             NOT NULL,
    [ThumbPath]        NVARCHAR (100)  NULL,
    [MediaPath]        NVARCHAR (100)  NOT NULL,
    [ShortDescription] NVARCHAR (100)  NULL,
    [Description]      NVARCHAR (1000) NOT NULL,
    [InsertedBy]       NVARCHAR (128)  NOT NULL,
    [InsertedDate]     DATETIME        NOT NULL,
    [IsActive]         BIT             NOT NULL,
    [IsDeleted]        BIT             NOT NULL,
    CONSTRAINT [PK_Media] PRIMARY KEY CLUSTERED ([MediaID] ASC)
);

