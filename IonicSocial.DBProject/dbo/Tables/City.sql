CREATE TABLE [dbo].[City] (
    [CityID]       INT           IDENTITY (1, 1) NOT NULL,
    [CityName]     NVARCHAR (50) NULL,
    [InsertedBy]   BIGINT        NULL,
    [InsertedDate] DATETIME      NULL,
    [IsActive]     BIT           NULL,
    [IsDeleted]    BIT           NULL,
    CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([CityID] ASC)
);

