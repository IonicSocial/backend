CREATE proc [dbo].[Proc_GetCenterListByLocation]
@LocationID int,
@latitude nvarchar(max),
@longitude nvarchar(max)
as

DECLARE @geoData geography;
SET @geoData = geography::STGeomFromText('POINT('+@latitude+' '+@longitude+')' , 4326);
SELECT  CenterID,CenterTitle,Address,ContactNumber,PinCode,Latitude,Longitude,
        @geoData.STDistance(st.GeogCol1) AS [DistanceFromPoint] -- (in meters)
     
    
FROM    CenterInformation st WITH(INDEX(SpatialIndex))
WHERE   @geoData.STDistance(st.GeogCol1) <= (Select CenterDistance*10000 From CenterSettings) and
 LocationID=@LocationID
ORDER BY @geoData.STDistance(st.GeogCol1) ASC

