/*
  FlexGCC Work Request Tracker - SQL Server setup script

  Run in SQL Server Management Studio (SSMS).
  Do not put real passwords or secrets in this file or commit them to GitHub.
*/

IF DB_ID(N'FlexGCCWorkTracker') IS NULL
BEGIN
    CREATE DATABASE FlexGCCWorkTracker;
END
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

IF OBJECT_ID(N'dbo.WorkRequestNotes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WorkRequestNotes
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WorkRequestNotes PRIMARY KEY,
        WorkRequestId INT NOT NULL,
        NoteText NVARCHAR(1000) NOT NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_WorkRequestNotes_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_WorkRequestNotes_WorkRequests
            FOREIGN KEY (WorkRequestId) REFERENCES dbo.WorkRequests(Id) ON DELETE CASCADE
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
