DROP PROC IF EXISTS sp_insertClassRunInternalValues
GO
Create PROC  sp_insertClassRunInternalValues(@classRunId uniqueidentifier, @classRunType varchar(50), @jsonValue varchar(max))
AS
Begin
    declare @i int;
    declare @length int;
    SET @i = 0;
        SET @length = 
        (SELECT COUNT(*) 
            FROM OPENJSON(@jsonValue))
        WHILE @i < @length
        BEGIN
            DECLARE @jsonValueItem NVARCHAR(100);
            SET @jsonValueItem = JSON_VALUE(@jsonValue,CONCAT('$[',@i,']'));
			DELETE dbo.ClassRunInternalValue WHERE ClassRunId = @classRunId and [Type] = @classRunType and [Value] = @jsonValueItem;
			insert into dbo.ClassRunInternalValue(ClassRunId,[Type], [Value]) values(@classRunId, @classRunType, @jsonValueItem)
            SET @i = @i +1;
        END
End
Go
IF Not EXISTS(SELECT * FROM dbo.ClassRunInternalValue
          WHERE Type in (
          'FacilitatorIds',
		  'CoFacilitatorIds'))
	Exec('
	DECLARE @id uniqueidentifier;

    DECLARE @FacilitatorIds varchar(max);
	DECLARE @CoFacilitatorIds varchar(max);

	----Create cursor for loop through Class Run table
	DECLARE cursorClassRunInternal CURSOR FOR SELECT
		id,

        FacilitatorIds,
		CoFacilitatorIds
	FROM dbo.ClassRun
	OPEN cursorClassRunInternal
	FETCH NEXT FROM cursorClassRunInternal INTO
		@id,

        @FacilitatorIds,
		@CoFacilitatorIds

	WHILE @@FETCH_STATUS = 0
	BEGIN
        EXEC sp_insertClassRunInternalValues @id, ''FacilitatorIds'', @FacilitatorIds;
		EXEC sp_insertClassRunInternalValues @id, ''CoFacilitatorIds'', @CoFacilitatorIds;

		FETCH NEXT FROM cursorClassRunInternal INTO
			@id,

            @FacilitatorIds,
			@CoFacilitatorIds
	END
	CLOSE cursorClassRunInternal;
	DEALLOCATE cursorClassRunInternal
	')
