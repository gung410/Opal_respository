-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Sarah, CXVN>
-- Create date: <2020-06-23T03:50:19>
-- Description:	<Use for getting department>
-- =============================================
CREATE OR ALTER FUNCTION [dbo].[GetPathDepartmentID] 
(
	@HDID as int
)
RETURNS varchar(200)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result varchar(200)
	Declare @ParentID as int
	Declare @HDDepartmentID as varchar(50)	

	-- Add the T-SQL statements to compute the return value here
	Select @ParentID = Parentid, @HDDepartmentID = h.DepartmentID 
    from org.H_D h (NOLOCK) 
    Where HDID = @HDID
  
	set @Result =  @HDDepartmentID + '\'	

	While @ParentID is not null 
	begin
 	 Select @ParentID = Parentid, @HDDepartmentID = h.DepartmentID 
     from org.H_D h (NOLOCK) 
     Where HDID = @ParentID

   	 set @Result =  @HDDepartmentID + '\' + @Result 	
	
	end

	-- Return the result of the function
	RETURN '\' + @Result

END
GO

