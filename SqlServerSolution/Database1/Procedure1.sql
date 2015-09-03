--https://msdn.microsoft.com/en-us/library/jj851226(v=vs.103).aspx
--https://msdn.microsoft.com/en-us/library/jj851212(v=vs.103).aspx
--http://blogs.msdn.com/b/ssdt/archive/2012/12/07/getting-started-with-sql-server-database-unit-testing-in-ssdt.aspx
CREATE PROCEDURE [dbo].[Procedure1]
	@param1 int = 0,
	@param2 int
AS
	SELECT @param1, @param2
RETURN 0
