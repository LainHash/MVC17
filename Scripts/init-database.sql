-- ============================================================
-- Scripts/init-database.sql
-- Database initialization script for new / fresh environments
-- ============================================================
-- Run this ONCE on a fresh SQL Server to create the DB and
-- restore from a backup (or scaffold the schema manually).
--
-- Usage (sqlcmd):
--   sqlcmd -S <host> -U sa -P <password> -i Scripts/init-database.sql
-- ============================================================

USE master;
GO

-- ── Step 1: Create the database if it does not exist ────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DBMVC05')
BEGIN
    CREATE DATABASE [DBMVC05];
    PRINT 'Database DBMVC05 created.';
END
ELSE
BEGIN
    PRINT 'Database DBMVC05 already exists — skipping creation.';
END
GO

-- ── Step 2: Restore from backup (uncomment to use) ──────────
-- Adjust the path to point to your .bak file.
-- This requires the SQL Server service account to have read
-- access to the backup file path.
--
-- USE master;
-- RESTORE DATABASE [DBMVC05]
--     FROM DISK = N'/var/opt/mssql/backup/DBMVC05_latest.bak'
--     WITH
--         MOVE N'DBMVC05'      TO N'/var/opt/mssql/data/DBMVC05.mdf',
--         MOVE N'DBMVC05_log'  TO N'/var/opt/mssql/data/DBMVC05_log.ldf',
--         REPLACE,
--         STATS = 10;
-- GO

-- ── Step 3: Create application login/user ──────────────────
-- Replace <APP_PASSWORD> with a strong password.
-- Run this only once; skip if user already exists.
--
-- USE master;
-- IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = N'mvc17_user')
-- BEGIN
--     CREATE LOGIN [mvc17_user] WITH PASSWORD = N'<APP_PASSWORD>';
-- END
-- GO
--
-- USE [DBMVC05];
-- IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = N'mvc17_user')
-- BEGIN
--     CREATE USER [mvc17_user] FOR LOGIN [mvc17_user];
--     ALTER ROLE db_datareader ADD MEMBER [mvc17_user];
--     ALTER ROLE db_datawriter ADD MEMBER [mvc17_user];
-- END
-- GO

PRINT 'Initialization script complete.';
GO
