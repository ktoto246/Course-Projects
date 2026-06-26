CREATE TABLE [dbo].[Status](
    [ID_Status] [int] IDENTITY(1,1) PRIMARY KEY,
    [Status] [nvarchar](40) NOT NULL
);
CREATE TABLE [dbo].[Edinica](
    [ID_Edinica] [int] IDENTITY(1,1) PRIMARY KEY,
    [DonorID] [int] NOT NULL,
    [Component] [nvarchar](10) NOT NULL,
    [FK_Status] [int] NOT NULL,
    [Date_Sbora] [date] NOT NULL,
    [Date_Freeze] [date] NULL,
    [BloodGroup] [nvarchar](10) NOT NULL,
    [Rh] [bit] NULL,
    CONSTRAINT [FK_Edinica_Status] FOREIGN KEY([FK_Status]) 
    REFERENCES [dbo].[Status]([ID_Status])
);
CREATE TABLE [dbo].[Compatibility_Erythrocytes](
    [Recipient_Group] [nvarchar](10) NOT NULL,
    [Recipient_Rh] [bit] NOT NULL,
    [Donor_Group] [nvarchar](10) NOT NULL,
    [Donor_Rh] [bit] NOT NULL,
    [IsCompatible] [bit] NOT NULL DEFAULT 0,
    PRIMARY KEY (Recipient_Group, Recipient_Rh, Donor_Group, Donor_Rh)
);
CREATE TABLE [dbo].[Compatibility_Plasma](
    [Recipient_Group] [nvarchar](10) NOT NULL,
    [Donor_Group] [nvarchar](10) NOT NULL,
    [IsCompatible] [bit] NOT NULL DEFAULT 0,
    PRIMARY KEY (Recipient_Group, Donor_Group)
);