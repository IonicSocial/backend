CREATE TABLE [dbo].[MediaType] (
    [MediaTypeID]  INT           IDENTITY (1, 1) NOT NULL,
    [Type]         NVARCHAR (50) NOT NULL,
    [InsertedBy]   BIGINT        NOT NULL,
    [InsertedDate] DATETIME      NOT NULL,
    [UpdatedBy]    BIGINT        NULL,
    [UpdatedDate]  DATETIME      NULL,
    [IsActive]     BIT           NOT NULL,
    [IsDeleted]    BIT           NOT NULL,
    CONSTRAINT [PK_MediaType] PRIMARY KEY CLUSTERED ([MediaTypeID] ASC)
);

