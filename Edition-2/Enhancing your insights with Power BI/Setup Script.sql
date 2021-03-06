/****** 

	CAUTION:
	
	Execute the steps in this script manually, it is not safe to run this directly at once.
	This script is only intended for testing purposes. Run at your own risk and responsibility.
	

	Introduction:

	Before executing this script make sure you have a database created called : Tfs_Warehouse_Extensions
	Make sure that reporting users have the appropriate permissions to create views / tables etc. A linked
	server registration is required that also needs permissions to be created. 
		
******/


USE [Tfs_Warehouse_Extensions]
GO

/****** 

	Create a support table that will containt the classifications that will be used to shape our report.
	In our example four classifications have been defined

		1: < Month
		2: 1-2 Months
		3: 3-6 Months
		4: > 6 Months

	The duration in days ranges from 1 - 1000
	The order is specified to make sorting available

******/
CREATE TABLE [dbo].[ClassificationHelper](
	[Classification] [nvarchar](50) NOT NULL,
	[DurationInDays] [int] NOT NULL,
	[SortOrder] [int] NOT NULL
) 
GO

/****** 
	Now open and execute the classification data helper script.
******/
GO

/****** 
	Create the Linked Server
	so data can be retrieved from your Tfs_Analysis database.
	Make sure permssions are valid!
******/

EXEC master.dbo.sp_addlinkedserver @server = N'cube_tfs', @srvproduct=N'', @provider=N'MSOLAP', @datasrc=N'X-JG-w12-sql14\tfs15', @catalog=N'tfs_analysis'
GO
EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname=N'cube_tfs',@useself=N'True',@locallogin=NULL,@rmtuser=NULL,@rmtpassword=NULL
GO


/****** 
	Create a view [dbo].[vw_Cube_PBIAndBugsOverTime]    
	This view is used to get data from analysis services. 
	There are numerous ways to create the Cube Query.	
	Use Analysis services tooling or PowerPivot or Power BI to create the query.	
******/
CREATE VIEW [dbo].[vw_Cube_PBIAndBugsOverTime]
AS
     
SELECT 

	"[Date].[Date].[Date].[MEMBER_CAPTION]"  as ActualDate,  
	"[Work Item].[System_WorkItemType].[System_WorkItemType].[MEMBER_CAPTION]"  as WorkitemType,
	"[Work Item].[System_State].[System_State].[MEMBER_CAPTION]" as States,
	"[Measures].[Work Item Count]" as Counts

FROM OPENQUERY(cube_tfs,' 

 SELECT NON EMPTY { [Measures].[Work Item Count] } ON COLUMNS, NON EMPTY { ([Date].[Date].[Date].ALLMEMBERS * [Work Item].[System_State].[System_State].ALLMEMBERS * [Work Item].[System_WorkItemType].[System_WorkItemType].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM ( SELECT ( { [Work Item].[System_WorkItemType].&[Bug], [Work Item].[System_WorkItemType].&[Product Backlog Item] } ) ON COLUMNS FROM ( SELECT ( { [Team Project].[Project Node Name].&[Agile Reporting] } ) ON COLUMNS FROM [Team System])) WHERE ( [Team Project].[Project Node Name].&[Agile Reporting] ) CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS

');	
GO

/****** 
	Create a View [dbo].[vw_PbiBugsOverTime]    
	This view transforms the original data by adding a duration in days to the results
******/
CREATE VIEW [dbo].[vw_PbiBugsOverTime]
AS
	SELECT 
		[ActualDate],
		[WorkitemType],
		[States],
		[Counts]
		, (SELECT DATEDIFF(day, cast( cast([ActualDate] as varchar(50)) as datetime), getdate())) as Duration
	FROM [dbo].[vw_Cube_PBIAndBugsOverTime]
GO

/****** 
	Create a View [dbo].[vw_PbiBugsOverTimeClassificationOrder]    
	This view uses the previous view to combine it with a classification	
******/
CREATE VIEW [dbo].[vw_PbiBugsOverTimeClassificationOrder]
AS

	SELECT * 
		, (select top 1 [sortorder] from ClassificationHelper where DurationInDays = duration) as SortOrder
	FROM vw_PbiBugsOverTimeClassification 
 
GO
