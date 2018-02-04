CREATE TABLE [dbo].[Location] (
    [LocationID]   INT           IDENTITY (1, 1) NOT NULL,
    [CityID]       INT           NOT NULL,
    [LocationName] NVARCHAR (50) NOT NULL,
    [InsertedBy]   BIGINT        NULL,
    [InsertedDate] DATETIME      NULL,
    [IsActive]     BIT           NULL,
    [IsDeleted]    BIT           NULL,
    CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([LocationID] ASC),
    CONSTRAINT [FK_Location_City] FOREIGN KEY ([CityID]) REFERENCES [dbo].[City] ([CityID])
);

