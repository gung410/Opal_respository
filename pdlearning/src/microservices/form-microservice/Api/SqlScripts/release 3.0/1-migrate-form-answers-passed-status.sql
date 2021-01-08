BEGIN TRANSACTION [MigrateFormAnswersPassedStatus]

BEGIN TRY
    /* 1. Update IsPassed status for Form Answers */
    /****** [Form]  ******/
	UPDATE [FormDb].[dbo].[FormAnswers]
	SET [PassingStatus] = IIF(
		ISNULL([Score], 0) < ISNULL([PassingMarkScore], 0) 
		OR ISNULL([ScorePercentage], 0) < ISNULL([PassingMarkPercentage], 0), 
		'Failed', 'Passed')
	FROM [FormDb].[dbo].[FormAnswers] AS FA LEFT JOIN [FormDb].[dbo].[Forms] AS F ON FA.[FormId] = F.[Id]
    WHERE FA.[IsCompleted] = 1

	COMMIT TRANSACTION [MigrateFormAnswersPassedStatus]

END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateFormAnswersPassedStatus]
END CATCH  

