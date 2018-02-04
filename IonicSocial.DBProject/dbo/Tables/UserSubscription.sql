CREATE TABLE [dbo].[UserSubscription] (
    [UserSubscriptionID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [SubscriptionType]   INT           NOT NULL,
    [StartDate]          DATETIME      NOT NULL,
    [EndDate]            DATETIME      NOT NULL,
    [OfferCode]          NVARCHAR (50) NULL,
    [InsertedBy]         BIGINT        NOT NULL,
    [InsertedDate]       DATETIME      NOT NULL,
    [UpdatedBy]          BIGINT        NULL,
    [UpdatedDate]        NCHAR (10)    NULL,
    [IsActive]           BIT           NOT NULL,
    [IsDeleted]          BIT           NOT NULL,
    CONSTRAINT [PK_UseSubscription] PRIMARY KEY CLUSTERED ([UserSubscriptionID] ASC)
);

