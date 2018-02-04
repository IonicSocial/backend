CREATE TABLE [dbo].[TextTo] (
    [TextToID]     BIGINT         IDENTITY (25, 1000) NOT NULL,
    [UserID]       NVARCHAR (128) NOT NULL,
    [TextTo]       NVARCHAR (500) NOT NULL,
    [InsertedBy]   NVARCHAR (128) NOT NULL,
    [InsertedDate] DATETIME       NOT NULL,
    [UpdatedBy]    NVARCHAR (128) NULL,
    [UpdatedDate]  DATETIME       NULL,
    [IsActive]     BIT            NOT NULL,
    [IsDeleted]    BIT            NOT NULL,
    CONSTRAINT [PK_TextTo] PRIMARY KEY CLUSTERED ([TextToID] ASC)
);

