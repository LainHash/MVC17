-- ============================================================
-- Scripts/backup-database.sql
-- Pre-deployment database backup script
-- ============================================================
-- Run this on the SQL Server BEFORE each deployment.
-- Adjust the backup path to match your server's drive layout.
--
-- Usage (sqlcmd):
--   sqlcmd -S localhost -U sa -P <password> -i Scripts/backup-database.sql
-- ============================================================

USE master;
GO

-- Generate a timestamped filename
DECLARE @BackupPath NVARCHAR(500);
DECLARE @Timestamp  NVARCHAR(20)  = REPLACE(REPLACE(CONVERT(NVARCHAR(20), GETDATE(), 120), ':', '-'), ' ', '_');

SET @BackupPath = N'C:\SQLBackups\DBMVC05_' + @Timestamp + N'.bak';

PRINT 'Starting backup to: ' + @BackupPath;

BACKUP DATABASE [DBMVC05]
    TO DISK = @BackupPath
    WITH
        FORMAT,                      -- overwrite existing backup sets in the file
        INIT,                        -- start a new media set
        NAME = N'DBMVC05-Full DB Backup',
        SKIP,
        NOREWIND,
        NOUNLOAD,
        COMPRESSION,
        STATS = 10;

PRINT 'Backup completed successfully.';
GO
