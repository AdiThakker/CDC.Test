use CDC
GO

create table Customer
(
CustomerId INT Identity(1,1) PRIMARY KEY,
[FirstName] varchar(100),
[LastName] varchar(100),
[Address] varchar(500),
[City] varchar(100),
[State] varchar(100),
[Country] varchar(100)
)

GO

CREATE TABLE dbo.[Outbox] (
    [ChangeId]     BIGINT             IDENTITY (1, 1) NOT NULL,
    [ActualLSN]    BINARY (10)        ,
    [EventType]    VARCHAR (500)      NULL,
    [EventTopic]   VARCHAR (500)      NULL,
	[EventBatchDate] DATETIMEOFFSET (7),
    [EventSentUTC] DATETIMEOFFSET (7) NULL,
    PRIMARY KEY CLUSTERED ([ChangeId] ASC)
)

GO

EXEC sys.sp_cdc_enable_db  
GO

EXEC sys.sp_cdc_enable_table  
		@source_schema = N'dbo',  
		@source_name   = N'Customer',  
		@role_name     = null,
		@supports_net_changes = 1
GO

INSERT INTO [dbo].[Customer]
           ([FirstName]
           ,[LastName]
           ,[Address]
           ,[City]
           ,[State]
           ,[Country])
     VALUES
           ('John'
           ,'Doe'
           ,'1234 Maple Dr.'
           ,'Chicago'
           ,'Illinois'
           ,'USA')
GO

USE [CDC]
GO
/****** Object:  StoredProcedure [dbo].[GetCDCUpdates]    Script Date: 10/22/2019 9:33:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetCDCUpdates]
AS
 
	DECLARE @EventBatchDate DATETIMEOFFSET
	SET @EventBatchDate = GETUTCDATE()

	DECLARE @MinLSN binary(10)
	DECLARE @MaxLSN binary(10)

	SELECT TOP 1 @MinLSN = [ActualLSN] FROM 
	dbo.Outbox
	WHERE EventSentUTC IS NOT NULL
	ORDER BY ChangeId DESC

	SET @MaxLSN = sys.fn_cdc_get_max_lsn()

	SELECT * into #CustomerCDC 
	FROM cdc.fn_cdc_get_all_changes_dbo_Customer(ISNULL(@MinLSN, sys.fn_cdc_get_min_lsn('dbo_Customer')), @MaxLSN, N'all');

	DELETE from #CustomerCDC where [__$start_lsn]= @MinLSN
	
	INSERT INTO dbo.Outbox
	(ActualLSN,  EventBatchDate) 
	SELECT DISTINCT [__$start_lsn], @EventBatchDate FROM (
	SELECT [__$start_lsn] FROM #CustomerCDC
	) lsns ORDER BY [__$start_lsn] ASC


	SELECT c.*, ChangeId FROM #CustomerCDC c JOIN dbo.Outbox o on c.[__$start_lsn] = o.ActualLSN WHERE EventBatchDate = @EventBatchDate

	Select ActualLSN from 
	dbo.Outbox 
	WHERE EventBatchDate = @EventBatchDate

	DROP TABLE #CustomerCDC

RETURN 0

