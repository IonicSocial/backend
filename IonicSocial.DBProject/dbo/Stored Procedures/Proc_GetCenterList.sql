
create proc [dbo].[Proc_GetCenterList]
 
as
 
SELECT  CenterID,CenterTitle,Address,ContactNumber,PinCode,Latitude,Longitude 
  
FROM    CenterInformation  

