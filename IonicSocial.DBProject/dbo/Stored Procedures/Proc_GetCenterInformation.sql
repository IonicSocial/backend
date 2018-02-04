
CREATE proc [dbo].[Proc_GetCenterInformation]
@CenterID int,
@latitude nvarchar(max),
@longitude nvarchar(max)
as
 

 

DECLARE @geoData geography;
SET @geoData = geography::STGeomFromText('POINT('+@latitude+' '+@longitude+')' , 4326);



SELECT  CenterID,CenterTitle,Address,ContactNumber,PinCode,Latitude,Longitude ,
    @geoData.STDistance(st.GeogCol1) AS [DistanceFromPoint] -- (in meters)

FROM    CenterInformation  st WITH(INDEX(SpatialIndex))

where  @geoData.STDistance(st.GeogCol1) <= (Select CenterDistance*1000 From CenterSettings)  and CenterID=@CenterID
ORDER BY @geoData.STDistance(st.GeogCol1) ASC
