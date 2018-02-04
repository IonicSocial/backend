CREATE TABLE [dbo].[SubscriptionType] (
    [SubscriptionTypeID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [SubsriptionType]    NVARCHAR (50) NOT NULL,
    [Price]              DECIMAL (18)  NOT NULL,
    [Duration]           TINYINT       NOT NULL,
    [OfferCode]          NVARCHAR (50) NULL,
    [InsertedDate]       DATETIME      NOT NULL,
    [InsertedBy]         BIGINT        NOT NULL,
    [UpdatedBy]          BIGINT        NULL,
    [UpdatedDate]        DATETIME      NULL,
    [IsActive]           BIT           NOT NULL,
    [IsDeleted]          BIT           NOT NULL,
    CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED ([SubscriptionTypeID] ASC)
);

