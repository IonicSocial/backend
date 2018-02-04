CREATE TABLE [dbo].[LocationInformation] (
    [CenterID]      INT               IDENTITY (1, 1) NOT NULL,
    [CityID]        INT               NOT NULL,
    [LocationID]    INT               NOT NULL,
    [CenterTitle]   NVARCHAR (50)     NULL,
    [Address]       NVARCHAR (500)    NULL,
    [ContactNumber] NVARCHAR (50)     NULL,
    [PinCode]       NVARCHAR (50)     NULL,
    [GeogCol1]      [sys].[geography] NULL,
    [GeogCol2]      AS                ([GeogCol1].[STAsText]()),
    [Latitude]      NVARCHAR (50)     NULL,
    [Longitude]     NVARCHAR (50)     NULL,
    [InsertedBy]    BIGINT            NULL,
    [InsertedDate]  DATETIME          NULL,
    [IsActive]      BIT               NULL,
    [IsDeleted]     BIT               NULL,
    CONSTRAINT [PK__SpatialT__3213E83F56F05C81] PRIMARY KEY CLUSTERED ([CenterID] ASC),
    CONSTRAINT [FK_CenterInformation_City] FOREIGN KEY ([CityID]) REFERENCES [dbo].[City] ([CityID]),
    CONSTRAINT [FK_CenterInformation_Location] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Location] ([LocationID])
);


GO
CREATE SPATIAL INDEX [Spatialindex]
    ON [dbo].[LocationInformation] ([GeogCol1])
    USING GEOGRAPHY_GRID
    WITH  (
            GRIDS = (LEVEL_1 = MEDIUM, LEVEL_2 = MEDIUM, LEVEL_3 = MEDIUM, LEVEL_4 = MEDIUM)
          );

