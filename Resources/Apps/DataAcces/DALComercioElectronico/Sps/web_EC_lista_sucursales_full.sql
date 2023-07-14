-- =============================================
-- Author:		<Author,,Zeruel>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

 create PROCEDURE [dbo].[web_EC_lista_sucursales_full]
	-- Add the parameters for the stored procedure here
	
	@UserTemp UNIQUEIDENTIFIER,
    @AppId UNIQUEIDENTIFIER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  
  select * from  [dbo].sucursales order by nombre
  
  



END

