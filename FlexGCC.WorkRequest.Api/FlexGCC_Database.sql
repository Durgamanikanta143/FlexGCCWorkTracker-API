/*
 FlexGCC Work Request Tracker - SQL Server setup script
 Run in SSMS. Never commit real passwords or secrets.
 Creates missing objects and expands an existing Description column to NVARCHAR(1000).
*/
IF DB_ID(N'FlexGCCWorkTracker') IS NULL
    CREATE DATABASE FlexGCCWorkTracker;
GO
USE FlexGCCWorkTracker;
GO

IF OBJECT_ID(N'dbo.WorkRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkRequests
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkRequests PRIMARY KEY,
        Title NVARCHAR(100) NOT NULL,
        ClientName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Priority NVARCHAR(20) NOT NULL CONSTRAINT DF_WorkRequests_Priority DEFAULT (N'Medium'),
        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_WorkRequests_Status DEFAULT (N'New'),
        DueDate DATE NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_WorkRequests_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT CK_WorkRequests_Priority CHECK (Priority IN (N'Low', N'Medium', N'High')),
        CONSTRAINT CK_WorkRequests_Status CHECK (Status IN (N'New', N'InProgress', N'Blocked', N'Completed'))
    );
END
GO

-- The supplied SSMS screenshot shows Description as NVARCHAR(100) (200 bytes).
-- Expand it safely to match the intended schema, NVARCHAR(1000) (2000 bytes).
IF COL_LENGTH(N'dbo.WorkRequests', N'Description') IS NOT NULL
   AND COL_LENGTH(N'dbo.WorkRequests', N'Description') < 2000
BEGIN
    ALTER TABLE dbo.WorkRequests ALTER COLUMN Description NVARCHAR(1000) NULL;
END
GO

IF OBJECT_ID(N'dbo.WorkRequestNotes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkRequestNotes
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkRequestNotes PRIMARY KEY,
        WorkRequestId INT NOT NULL,
        NoteText NVARCHAR(1000) NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_WorkRequestNotes_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_WorkRequestNotes_WorkRequests
            FOREIGN KEY (WorkRequestId) REFERENCES dbo.WorkRequests(Id)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_WorkRequests_Status' AND object_id = OBJECT_ID(N'dbo.WorkRequests'))
    CREATE INDEX IX_WorkRequests_Status ON dbo.WorkRequests(Status);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_WorkRequests_ClientName' AND object_id = OBJECT_ID(N'dbo.WorkRequests'))
    CREATE INDEX IX_WorkRequests_ClientName ON dbo.WorkRequests(ClientName);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_WorkRequestNotes_WorkRequestId' AND object_id = OBJECT_ID(N'dbo.WorkRequestNotes'))
    CREATE INDEX IX_WorkRequestNotes_WorkRequestId ON dbo.WorkRequestNotes(WorkRequestId);
GO

SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
  AND TABLE_NAME IN ('WorkRequests', 'WorkRequestNotes');
GO
SELECT c.name AS ColumnName, TYPE_NAME(c.user_type_id) AS DataType,
       c.max_length AS MaxLengthBytes, c.is_nullable AS IsNullable
FROM sys.columns c
WHERE c.object_id = OBJECT_ID(N'dbo.WorkRequests')
  AND c.name = N'Description';
GO
