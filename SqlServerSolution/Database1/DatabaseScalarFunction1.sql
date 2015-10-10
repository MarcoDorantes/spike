CREATE FUNCTION [dbo].[DatabaseScalarFunction1]
(
	@param1 int,
	@param2 int
)
RETURNS INT
AS
BEGIN
	RETURN @param1 + @param2
END
/*
USE db_;
GO
IF OBJECT_ID (N'dbo.GetSolSenderClientConfiguredSchemaAndHostNameOrIPAndPort', N'FN') IS NOT NULL
    DROP FUNCTION dbo.GetSolSenderClientConfiguredSchemaAndHostNameOrIPAndPort
GO
CREATE FUNCTION dbo.GetSolSenderClientConfiguredSchemaAndHostNameOrIPAndPort (@vpn VARCHAR(250))
RETURNS VARCHAR(250)
WITH EXECUTE AS CALLER
AS
BEGIN
     RETURN('https://hostname:7000/');
END;
SELECT dbo.ISOweek(CONVERT(DATETIME,'12/26/2004',101)) AS 'ISO Week';
*/