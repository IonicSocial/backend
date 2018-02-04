

create proc [dbo].[Proc_UpdateCenterInformation]

@CenterTitle nvarchar(50),
@Address nvarchar(500),
@ContactNumber nvarchar(50),
@PinCode nvarchar(50),
@Latitude nvarchar(50),
@Longitude nvarchar(50),
@CenterID int
as

Update  CenterInformation

Set CenterTitle=@CenterTitle,
Address=@Address,
ContactNumber=@ContactNumber,
PinCode=@PinCode,
Latitude=@Latitude,
Longitude=@Longitude,
GeogCol1 =geography::STGeomFromText('POINT('+@Latitude+' '+@Longitude+')',4326)
where CenterID=@CenterID



