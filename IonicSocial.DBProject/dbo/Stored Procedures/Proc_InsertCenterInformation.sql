

CREATE proc [dbo].[Proc_InsertCenterInformation]
@CityID int,
@LocationID int,
@CenterTitle nvarchar(50),
@Address nvarchar(500),
@ContactNumber nvarchar(50),
@PinCode nvarchar(50),
@Latitude nvarchar(50),
@Longitude nvarchar(50)
as

INSERT INTO CenterInformation(CityID,LocationID,CenterTitle,Address,ContactNumber,PinCode,Latitude,Longitude,GeogCol1)
VALUES 
(@CityID,@LocationID,@CenterTitle,@Address,@ContactNumber,@PinCode,@Latitude,@Longitude,geography::STGeomFromText('POINT('+@Latitude+' '+@Longitude+')',4326))
