create PROCEDURE [dbo].[web_EC_obtener_horarios_sucursal] -- Add the parameters for the stored procedure here
@UserTemp UNIQUEIDENTIFIER,
 @AppId UNIQUEIDENTIFIER,
 @id_sucursal INT,
 @Insertado_por nvarchar (MAX),
 @fecha_Insertado datetime2 AS
BEGIN


declare @exist INT
declare @id INT

select @exist =  count(*) 
from sucursal_horarios
where 
id_sucursal = @id_sucursal


select @id =  id
from sucursal_horarios
where 
id_sucursal = @id_sucursal



	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	if @exist>0 
	begin
		

select top 1 *
from sucursal_horarios
where 
id_sucursal = @id_sucursal

   

	

		END
		else 
	begin
		
	INSERT INTO [Commerce2].[dbo].[sucursal_horarios] (
		
		[id_sucursal],
		[horarios],
		[modificado_por],
		[fecha_modificacion],
		[Insertado_por],
		[fecha_Insertado]
	)
VALUES
	(
		
		 @id_sucursal,
		N'',
		@Insertado_por,
		 @fecha_Insertado,
		@Insertado_por,
		 @fecha_Insertado
	) ;




select top 1 *
from sucursal_horarios
where 
id_sucursal = @id_sucursal


	end







END
