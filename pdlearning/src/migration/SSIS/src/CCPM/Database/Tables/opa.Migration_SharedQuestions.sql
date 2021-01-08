IF OBJECT_ID(N'opa.Migration_SharedQuestions', N'U') IS NULL
BEGIN
CREATE TABLE opa.Migration_SharedQuestions(
	Id uniqueidentifier NOT NULL,
	CreatedDate datetime2(7) NOT NULL,
	ChangedDate datetime2(7) NULL,
	ExternalId varchar(255) NULL,
	CreatedBy uniqueidentifier NOT NULL,
	ChangedBy uniqueidentifier NULL,
	Question_Type varchar(30) NOT NULL,
	Question_Title ntext NULL,
	Question_CorrectAnswer nvarchar(max) NULL,
	Question_Options nvarchar(max) NULL,
	Question_Hint nvarchar(max) NULL,
	Question_AnswerExplanatoryNote nvarchar(max) NULL,
	Question_Level int NULL,
	OwnerId uniqueidentifier NOT NULL,
	IsDeleted bit NOT NULL,
	PRIMARY KEY (Id )
)
END