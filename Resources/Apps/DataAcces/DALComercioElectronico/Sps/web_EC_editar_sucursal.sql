-- Description:	<Description,,>
-- =============================================

 create PROCEDURE [dbo].[web_EC_editar_sucursal]
	-- Add the parameters for the stored procedure here
	
	@UserTemp UNIQUEIDENTIFIER,
    @AppId UNIQUEIDENTIFIER,
@id_sucursal int ,
@id_sucursal_madre int =null,

@clave nvarchar (max) =NULL ,
@secuencia int = NULL ,
@nombre nvarchar (max) ,
@path_imagen nvarchar  (max),
@coordenadas nvarchar (max) ,
@responsable nvarchar (max) ,
@calle nvarchar  (max),
@colonia nvarchar(max),
@ciudad nvarchar(max)  ,
@estado nvarchar(max),
@cp int  ,
@telefono nvarchar (max) ,
@activa bit  ,
@cargo_envio decimal(10,2) = NULL ,
@minimo_para_entrega decimal(10,2)= NULL ,
@modificado_por nvarchar(max) ,
@fecha_modificacion datetime2  ,
--@Insertado_por nvarchar(100)  ,
--@fecha_Insertado datetime2  ,
@URL_PuntoVenta varchar(max) = NULL 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

  
 




UPDATE [dbo].[sucursales] 
set 	[id_sucursal_madre]=		@id_sucursal_madre,
	[clave]=		@clave,
	[secuencia]=	@secuencia,
	[nombre]=@nombre,
	[path_imagen]=@path_imagen,
	[coordenadas]=	@coordenadas,
	[responsable]=@responsable,
	[calle]=@calle,
	[colonia]=@colonia,
	[ciudad]=@ciudad,
	[estado]=@estado,
	[cp]=@cp,
	[telefono]=@telefono,
	[activa]=@activa,
	[cargo_envio]=@cargo_envio,
	[minimo_para_entrega]=@minimo_para_entrega,
	[modificado_por]=@modificado_por,
	[fecha_modificacion]=@fecha_modificacion,
	--[Insertado_por]=@Insertado_por,
	--[fecha_Insertado]=@fecha_Insertado,
	[URL_PuntoVenta]=@URL_PuntoVenta



where id_sucursal=@id_sucursal	




  



END

