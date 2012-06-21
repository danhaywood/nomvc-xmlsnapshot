
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 06/21/2012 13:43:20
-- Generated from EDMX file: D:\GITHUB\danhaywood\nomvc-xmlsnapshot\Demo.Dom.Claims\Claims.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [XmlSnapshot];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RecordedActionForClaim_Claim]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RecordedActionForClaims] DROP CONSTRAINT [FK_RecordedActionForClaim_Claim];
GO
IF OBJECT_ID(N'[dbo].[FK_Claim_Allowance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Allowances] DROP CONSTRAINT [FK_Claim_Allowance];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Claims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Claims];
GO
IF OBJECT_ID(N'[dbo].[RecordedActionForClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RecordedActionForClaims];
GO
IF OBJECT_ID(N'[dbo].[Allowances]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Allowances];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Claims'
CREATE TABLE [dbo].[Claims] (
    [Id] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RecordedActionForClaims'
CREATE TABLE [dbo].[RecordedActionForClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Summary] nvarchar(max)  NOT NULL,
    [Rationale] nvarchar(max)  NOT NULL,
    [User] nvarchar(max)  NOT NULL,
    [XmlSnapshot] nvarchar(max)  NOT NULL,
    [Claim_Id] int  NOT NULL
);
GO

-- Creating table 'Allowances'
CREATE TABLE [dbo].[Allowances] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ClaimId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Claims'
ALTER TABLE [dbo].[Claims]
ADD CONSTRAINT [PK_Claims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RecordedActionForClaims'
ALTER TABLE [dbo].[RecordedActionForClaims]
ADD CONSTRAINT [PK_RecordedActionForClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Allowances'
ALTER TABLE [dbo].[Allowances]
ADD CONSTRAINT [PK_Allowances]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Claim_Id] in table 'RecordedActionForClaims'
ALTER TABLE [dbo].[RecordedActionForClaims]
ADD CONSTRAINT [FK_RecordedActionForClaim_Claim]
    FOREIGN KEY ([Claim_Id])
    REFERENCES [dbo].[Claims]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RecordedActionForClaim_Claim'
CREATE INDEX [IX_FK_RecordedActionForClaim_Claim]
ON [dbo].[RecordedActionForClaims]
    ([Claim_Id]);
GO

-- Creating foreign key on [ClaimId] in table 'Allowances'
ALTER TABLE [dbo].[Allowances]
ADD CONSTRAINT [FK_ClaimAllowance]
    FOREIGN KEY ([ClaimId])
    REFERENCES [dbo].[Claims]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClaimAllowance'
CREATE INDEX [IX_FK_ClaimAllowance]
ON [dbo].[Allowances]
    ([ClaimId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------