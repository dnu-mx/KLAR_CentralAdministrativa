-- Description:	<Description,,>
-- =============================================

 create PROCEDURE [dbo].[web_EC_agregar_sucursal]
	-- Add the parameters for the stored procedure here
	
	@UserTemp UNIQUEIDENTIFIER,
    @AppId UNIQUEIDENTIFIER,
@id_sucursal_madre int =null,

@clave nvarchar (max) =NULL ,
@secuencia int = NULL ,
@nombre nvarchar (max)  ,
@path_imagen nvarchar (max)  ,
@coordenadas nvarchar (max)  ,
@responsable nvarchar (max)  ,
@calle nvarchar (max)  ,
@colonia nvarchar (max),
@ciudad nvarchar (max)  ,
@estado nvarchar (max),
@cp int  ,
@telefono nvarchar (max)  ,
@activa bit  ,
@cargo_envio decimal(10,2) = NULL ,
@minimo_para_entrega decimal(10,2)= NULL ,
@modificado_por nvarchar (max) ,
@fecha_modificacion datetime2  ,
@Insertado_por nvarchar (max)  ,
@fecha_Insertado datetime2  ,
@URL_PuntoVenta varchar = NULL 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  
 




INSERT INTO [dbo].[sucursales] (
	[id_sucursal_madre],
	[clave],
	[secuencia],
	[nombre],
	[path_imagen],
	[coordenadas],
	[responsable],
	[calle],
	[colonia],
	[ciudad],
	[estado],
	[cp],
	[telefono],
	[activa],
	[cargo_envio],
	[minimo_para_entrega],
	[modificado_por],
	[fecha_modificacion],
	[Insertado_por],
	[fecha_Insertado],
	[URL_PuntoVenta]
)
VALUES
	(
		@id_sucursal_madre,
		@clave,
		@secuencia,
		@nombre,
		'path',--@path_imagen,
		@coordenadas,
	'',--@responsable,
	@calle,
	@colonia,
	@ciudad,
	@estado,
	@cp,
	@telefono,
	@activa,
	@cargo_envio,
	@minimo_para_entrega,
	@modificado_por,
	@fecha_modificacion,
	@Insertado_por,
	@fecha_Insertado,
	@URL_PuntoVenta
	);



  



END

