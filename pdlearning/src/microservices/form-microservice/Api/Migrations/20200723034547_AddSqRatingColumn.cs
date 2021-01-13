using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Form.Migrations
{
    public partial class AddSqRatingColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Satisfaction",
                table: "FormAnswers",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SqRating",
                table: "FormAnswers",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Usefulness",
                table: "FormAnswers",
                nullable: true);
            migrationBuilder.Sql(
            @"if (select count(*) from Forms 
                where Id in ('84A223BD-D40A-4D39-8D2A-9350970F7F87','3FBBB60B-E749-45F6-8F58-62D49C928DF3','0FCF4D43-5131-487F-8FE1-CA08AB817293','5BCF5F78-37A8-4BD5-91E5-B73F9D3DB2EB')) = 4
                begin
                    update Forms
                    set SqRatingType =(case
                        when Id='84A223BD-D40A-4D39-8D2A-9350970F7F87' then 'CourseWorkshopMasterclassSeminarConference'
                        when Id='3FBBB60B-E749-45F6-8F58-62D49C928DF3' then 'ELearningCourse'
                        when Id='0FCF4D43-5131-487F-8FE1-CA08AB817293' then 'BlendedCourseWorkshopMasterclass'
                        when Id='5BCF5F78-37A8-4BD5-91E5-B73F9D3DB2EB' then 'LearningEvent'
                        end)
		
                    update FormQuestions
                    set Question_IsSurveyTemplateQuestion = 1
                    where FormId in ('84A223BD-D40A-4D39-8D2A-9350970F7F87','3FBBB60B-E749-45F6-8F58-62D49C928DF3','0FCF4D43-5131-487F-8FE1-CA08AB817293','5BCF5F78-37A8-4BD5-91E5-B73F9D3DB2EB')
                end
            else
                begin
                    declare @type1Id varchar(50),@type2Id varchar(50),@type3Id varchar(50),@type4Id varchar(50)
                    set @type1Id='84A223BD-D40A-4D39-8D2A-9350970F7F87'
                    set @type2Id='3FBBB60B-E749-45F6-8F58-62D49C928DF3'
                    set @type3Id='0FCF4D43-5131-487F-8FE1-CA08AB817293'
                    set @type4Id='5BCF5F78-37A8-4BD5-91E5-B73F9D3DB2EB'

                    insert into Forms(Id,CreatedDate,ChangedDate,CreatedBy,Title,[Type],[Status],OwnerId,IsDeleted,RandomizedQuestions,IsArchived,OriginalObjectId,ParentId,SurveyType,DepartmentId,IsSurveyTemplate,AnswerFeedbackDisplayOption,SqRatingType) 
                    values(@type1Id,GETDATE(),GETDATE(),'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3','Type 1: FEEDBACK FORM FOR COURSE/WORKSHOP/MASTER CLASS/SEMINAR/CONFERENCE','Survey','Published','DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3',0,0,0,@type1Id,'00000000-0000-0000-0000-000000000000','PostCourse',1,1,'AfterAnsweredQuestion','CourseWorkshopMasterclassSeminarConference')
                    insert into Forms(Id,CreatedDate,ChangedDate,CreatedBy,Title,[Type],[Status],OwnerId,IsDeleted,RandomizedQuestions,IsArchived,OriginalObjectId,ParentId,SurveyType,DepartmentId,IsSurveyTemplate,AnswerFeedbackDisplayOption,SqRatingType)
                    values(@type2Id,GETDATE(),GETDATE(),'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3','Type 2: FEEDBACK FORM FOR E-LEARNING COURSE','Survey','Published','DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3',0,0,0,@type2Id,'00000000-0000-0000-0000-000000000000','PostCourse',1,1,'AfterAnsweredQuestion','ELearningCourse')
                    insert into Forms(Id,CreatedDate,ChangedDate,CreatedBy,Title,[Type],[Status],OwnerId,IsDeleted,RandomizedQuestions,IsArchived,OriginalObjectId,ParentId,SurveyType,DepartmentId,IsSurveyTemplate,AnswerFeedbackDisplayOption,SqRatingType)
                    values(@type3Id,GETDATE(),GETDATE(),'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3','Type 3: FEEDBACK FORM FOR BLENDED COURSE/WORKSHOP/MASTER CLASS','Survey','Published','DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3',0,0,0,@type3Id,'00000000-0000-0000-0000-000000000000','PostCourse',1,1,'AfterAnsweredQuestion','BlendedCourseWorkshopMasterclass')
                    insert into Forms(Id,CreatedDate,ChangedDate,CreatedBy,Title,[Type],[Status],OwnerId,IsDeleted,RandomizedQuestions,IsArchived,OriginalObjectId,ParentId,SurveyType,DepartmentId,IsSurveyTemplate,AnswerFeedbackDisplayOption,SqRatingType)
                    values(@type4Id,GETDATE(),GETDATE(),'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3','Type 4: FEEDBACK FORM FOR LEARNING EVENT','Survey','Published','DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3',0,0,0,@type4Id,'00000000-0000-0000-0000-000000000000','PostCourse',1,1,'AfterAnsweredQuestion','LearningEvent')

                    insert into FormQuestions(Id,CreatedDate,ChangedDate,CreatedBy,ChangedBy,Question_Type,Question_Title,Question_Options,FormId,Title,[Priority],RandomizedOptions,Score,IsDeleted,Question_IsSurveyTemplateQuestion)
                    values(NEWID(),GETDATE(),GETDATE(),'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3','DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3','SingleChoice','I can apply the ideas/knowledge/skills learnt from the e- learning course.','[{""Code"":1, ""Value"":""Strongly Disagree"", ""Feedback"":null, ""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]',@type2Id,'I can apply the ideas/knowledge/skills learnt from the e- learning course.',3,0,1,0,1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'Some ideas/comments that I would like to share with the course provider:', null, @type2Id, 'Some ideas/comments that I would like to share with the course provider:', 6, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I would recommend the e-learning course to my colleagues.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'I would recommend the e-learning course to my colleagues.', 5, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The objectives were achieved.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'The objectives were achieved.', 0, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The learning resources supported me in my learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'The learning resources supported me in my learning.', 1, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The presentation was clear.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'The presentation was clear.', 2, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', NULL, 'TrueFalse', 'I would like to share my identity with the course provider.', '[{""Code"":1,""Value"":true,""Feedback"":null,""Type"":null},{""Code"":2,""Value"":false,""Feedback"":null,""Type"":null}]', @type2Id, 'I would like to share my identity with the course provider.', 7, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The e-Learning course met my learning needs.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'The e-Learning course met my learning needs.', 4, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The duration of the course / workshop / master class / seminar / conference was sufficient to meet its objectives.', '[{""Code"":0,""Value"":""Too short"",""Feedback"":null,""Type"":null},{""Code"":1,""Value"":""Just right"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Too long"",""Feedback"":null,""Type"":null}]', @type2Id, 'The duration of the course / workshop / master class / seminar / conference was sufficient to meet its objectives.', 9, 0, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'Some ways that this learning experience can be improved:', null, @type2Id, 'Some ways that this learning experience can be improved:', 9, NULL, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I can apply the ideas / knowledge / skills learnt from the course / workshop / master class / seminar / conference.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'I can apply the ideas / knowledge / skills learnt from the course / workshop / master class / seminar / conference.', 7, 0, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', '7 The course / workshop / master class / seminar / conference helped me to reflect on my teaching practices. ', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type2Id, '7 The course / workshop / master class / seminar / conference helped me to reflect on my teaching practices. ', 6, 0, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I would recommend the course / workshop/ master class / seminar / conference to my colleagues. ', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null}]', @type2Id, 'I would recommend the course / workshop/ master class / seminar / conference to my colleagues. ', 8, 0, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'TrueFalse', 'I would like to share my identity with the course provider.', '[{""Code"":1,""Value"":true,""Feedback"":null,""Type"":null},{""Code"":2,""Value"":false,""Feedback"":null,""Type"":null}]', @type1Id, 'I would like to share my identity with the course provider.', 12, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The presentation was clear.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The presentation was clear.', 2, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I would recommend the course / workshop/ master class / seminar / conference to my colleagues.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'I would recommend the course / workshop/ master class / seminar / conference to my colleagues.', 8, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The facilitator is skilful at facilitating the participants’ learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The facilitator is skilful at facilitating the participants’ learning.', 4, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The objectives were achieved.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The objectives were achieved.', 0, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The questions were adequately addressed at the course / workshop / master class / seminar / conference.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The questions were adequately addressed at the course / workshop / master class / seminar / conference.', 3, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I can apply the ideas / knowledge / skills learnt from the course / workshop / master class / seminar / conference.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'I can apply the ideas / knowledge / skills learnt from the course / workshop / master class / seminar / conference.', 7, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The course / workshop / master class / seminar / conference met my learning needs.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The course / workshop / master class / seminar / conference met my learning needs.', 5, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The duration of the course / workshop / master class / seminar / conference was sufficient to meet its objectives.', '[{""Code"":0,""Value"":""Too short"",""Feedback"":null,""Type"":null},{""Code"":1,""Value"":""Just right"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Too long"",""Feedback"":null,""Type"":null}]', @type1Id, 'The duration of the course / workshop / master class / seminar / conference was sufficient to meet its objectives.', 9, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'Some ways that this learning experience can be improved:', null, @type1Id, 'Some ways that this learning experience can be improved:', 11, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The learning resources supported me in my learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The learning resources supported me in my learning.', 1, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'Some useful idea(s) from the course/workshop/master class/seminar/conference which I would like to apply:', null, @type1Id, 'Some useful idea(s) from the course/workshop/master class/seminar/conference which I would like to apply:', 10, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The course / workshop / master class / seminar / conference helped me to reflect on my teaching practices.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type1Id, 'The course / workshop / master class / seminar / conference helped me to reflect on my teaching practices.', 6, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'I would like to share my identity with the course provider.', null, @type1Id, 'I would like to share my identity with the course provider.', 12, NULL, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The duration of the learning event was sufficient to meet its objectives.', '[{""Code"":0,""Value"":""Too short"",""Feedback"":null,""Type"":null},{""Code"":1,""Value"":""Just right"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Too long"",""Feedback"":null,""Type"":null}]', @type4Id, 'The duration of the learning event was sufficient to meet its objectives.', 8, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'Aspects of this learning event that have been well-designed:', null, @type4Id, 'Aspects of this learning event that have been well-designed:', 9, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The learning event met my learning needs.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'The learning event met my learning needs.', 3, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I would recommend the learning event to my colleagues.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'I would recommend the learning event to my colleagues.', 5, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The presentation was clear.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'The presentation was clear.', 2, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I can apply the ideas / knowledge / skills learnt from the learning event.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'I can apply the ideas / knowledge / skills learnt from the learning event.', 4, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'ShortText', 'Ways that this learning event can be improved:', null, @type4Id, 'Ways that this learning event can be improved:', 10, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', NULL, 'TrueFalse', 'I would like to share my identity with the organiser.', '[{""Code"":1,""Value"":true,""Feedback"":null,""Type"":null},{""Code"":2,""Value"":false,""Feedback"":null,""Type"":null}]', @type4Id, 'I would like to share my identity with the organiser.', 11, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The objectives were achieved.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'The objectives were achieved.', 0, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The following activities facilitated my learning:', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'The following activities facilitated my learning:', 6, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The learning resources supported me in my learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'The learning resources supported me in my learning.', 1, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I can apply the ideas/knowledge/skills learnt from the blended course/workshop/master class.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'I can apply the ideas/knowledge/skills learnt from the blended course/workshop/master class.', 7, 0, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I would recommend the blended course/workshop/master class to my colleagues.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type4Id, 'I would recommend the blended course/workshop/master class to my colleagues.', 8, 0, 1, 1, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The face-to-face interactions during the blended course / workshop / master class facilitated me in my learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The face-to-face interactions during the blended course / workshop / master class facilitated me in my learning.', 3, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The presentation was clear.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The presentation was clear.', 2, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The online interactions during the blended course / workshop / master class facilitated me in my learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The online interactions during the blended course / workshop / master class facilitated me in my learning.', 4, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', NULL, 'ShortText', 'Some ways that this learning experience can be improved:', null, @type3Id, 'Some ways that this learning experience can be improved:', 11, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The blended course / workshop / master class met my learning needs.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The blended course / workshop / master class met my learning needs.', 5, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The interactions during the blended course / workshop / master class helped me to reflect on my classroom practices.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The interactions during the blended course / workshop / master class helped me to reflect on my classroom practices.', 6, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The objectives were achieved.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The objectives were achieved.', 0, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The learning resources supported me in my learning.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'The learning resources supported me in my learning.', 1, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', NULL, 'TrueFalse', 'I would like to share my identity with the course provider.', '[{""Code"":1,""Value"":true,""Feedback"":null,""Type"":null},{""Code"":2,""Value"":false,""Feedback"":null,""Type"":null}]', @type3Id, 'I would like to share my identity with the course provider.', 12, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I can apply the ideas/ knowledge/ skills learnt from the blended course / workshop / master class.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'I can apply the ideas/ knowledge/ skills learnt from the blended course / workshop / master class.', 7, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', NULL, 'ShortText', 'Some useful idea(s) from the blended course/workshop/master class which I would like to apply:', null, @type3Id, 'Some useful idea(s) from the blended course/workshop/master class which I would like to apply:', 10, NULL, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'The duration of the blended course / workshop / master class was sufficient to meet its objectives.', '[{""Code"":0,""Value"":""Too short"",""Feedback"":null,""Type"":null},{""Code"":1,""Value"":""Just right"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Too long"",""Feedback"":null,""Type"":null}]', @type3Id, 'The duration of the blended course / workshop / master class was sufficient to meet its objectives.', 9, 0, 1, 0, 1),
                    (NEWID(), GETDATE(), GETDATE(), 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'DE1F5BF7-E924-4BF6-9C02-EDEBD8456FD3', 'SingleChoice', 'I would recommend the blended course / workshop / master class to my colleagues.', '[{""Code"":1,""Value"":""Strongly Disagree"",""Feedback"":null,""Type"":null},{""Code"":2,""Value"":""Disagree"",""Feedback"":null,""Type"":null},{""Code"":3,""Value"":""Agree"",""Feedback"":null,""Type"":null},{""Code"":4,""Value"":""Strongly Agree"",""Feedback"":null,""Type"":null}]', @type3Id, 'I would recommend the blended course / workshop / master class to my colleagues.', 8, 0, 1, 0, 1)
                end");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Satisfaction",
                table: "FormAnswers");

            migrationBuilder.DropColumn(
                name: "SqRating",
                table: "FormAnswers");

            migrationBuilder.DropColumn(
                name: "Usefulness",
                table: "FormAnswers");
        }
    }
}