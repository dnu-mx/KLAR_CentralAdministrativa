
 create PROCEDURE [dbo].[web_EC_actualizar_horarios_sucursal]
	-- Add the parameters for the stored procedure here
	
	@UserTemp UNIQUEIDENTIFIER,
    @AppId UNIQUEIDENTIFIER,
@id_sucursal int ,
@id int ,


@horarios nvarchar (max)  ,


@modificado_por nvarchar(max) ,
@fecha_modificacion datetime2  

as

BEGIN





UPDATE [dbo].[sucursal_horarios] 
set 

	[horarios] = @horarios ,
	[modificado_por]=@modificado_por,
	[fecha_modificacion]=@fecha_modificacion

	--[Insertado_por]=@Insertado_por,
	--[fecha_Insertado]=@fecha_Insertado,
	



where id_sucursal=@id_sucursal	
 and id=@id

  



END

