using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Producer
{
    static readonly ConnectionFactory factory;
    static readonly IConnection connection;
    static readonly IModel channel;
    static Producer()
    {
        factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://guest:guest@localhost");
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
    }
    public static void Main()
    {
        Console.WriteLine("Press [A] to InsertEventRating. Press [B] to InsertAggregationRating. Press [ESC] to exit.");
        do
        {
            while (!Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.A:
                        InsertEventRatingFail();
                        break;
                    case ConsoleKey.B:
                        InsertAggregationRatingSuccess();
                        break;
                    case ConsoleKey.C:
                        InsertCommandInsertUserSuccess();
                        break;
                    case ConsoleKey.D:
                        InsertEventTimeUsedSuccess();
                        break;
                    case ConsoleKey.E:
                        InsertUserEventSuccess();
                        break;
                    case ConsoleKey.H:
                        UpdateUserEventSuccess();
                        break;
                    case ConsoleKey.I:
                        DeleteUserEventSuccess();
                        break;
                    case ConsoleKey.F:
                        InsertOrgSuccess();
                        break;
                    case ConsoleKey.G:
                        InsertUserCommandSuccess();
                        break;
                    case ConsoleKey.J:
                        InsertOrgUnitUserSuccess();
                        break;
                    case ConsoleKey.K:
                        DeleteOrgUnitUserSuccess();
                        break;
                    case ConsoleKey.L:
                        InsertSnapShotSuccess();
                        break;
                    default:
                        break;
                }


            }
        } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        Console.ReadLine();
    }

    private static void InsertAggregationRatingSuccess()
    {
        Console.WriteLine(nameof(InsertAggregationRatingSuccess));
        var json = "{\r\n  \"type\": \"event\",\r\n  \"version\": \"1.0\",\r\n  \"created\": \"2018-08-21T18:25:43.511Z\",\r\n  \"id\": \"05ac9834-5421-44a5-ae81-3596ceff6ca0\",\r\n  \"routing\": {\r\n    \"action\": \"learnapp.crud.create.rating\",\r\n    \"actionVersion\": \"1.2\",\r\n    \"entity\": \"coursepad.course.courseobject\",\r\n    \"entityId\": \"{course-id}\"\r\n  },\r\n  \"payload\": {\r\n    \"identity\": {\r\n      \"clientId\": \"random('232','28','33')\",\r\n      \"customerId\": \"12\",\r\n      \"sourceIp\": \"stringMerge(.,10.0.0,cur.clientId)\",\r\n      \"userId\": \"uuid()\",\r\n      \"onBehalfOfUser\": \"uuid()\"\r\n    },\r\n    \"references\": {\r\n      \"externalId\": \"ui-user-navigation-id-33\",\r\n      \"correlationId\": \"uuid()\",\r\n      \"commandId\": \"uuid()\",\r\n      \"eventId\": \"uuid()\"\r\n    },\r\n    \"body\": {\r\n      \"course_id\": \"random('11','22','33','44','55','66','77','88','99')\",\r\n      \"rate\": \"4\",\r\n      \"comment\": \"alphaNumeric(30)\",\r\n      \"rating_date\": \"nowTimestamp()\",\r\n      \"reviewer\": \"firstName()\"\r\n    }\r\n  }\r\n}";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "learnapp.crud.create.rating",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertEventRatingFail()
    {
        Console.WriteLine(nameof(InsertEventRatingFail));
        var json = "{\"message\":\"Executing ObjectResult, writing value of type '\\\"null\\\"'.\",\"EventId\":{\"Id\":1},\"SourceContext\":\"Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor\",\"@timestamp\":\"2018-10-26T10:59:11.009Z\",\"Type\":\"null\",\"messageTemplate\":\"Executing ObjectResult, writing value of type '{Type}'.\",\"ActionId\":\"c09a7448-7ca3-4bd1-ad6d-80bb8dda6989\",\"@version\":\"1\",\"level\":\"Information\",\"ActionName\":\"Data.Intergration.Api.Controllers.ImportUdemyController.TriggerGetUdemyCourses (Data.Intergration.Api)\",\"Application\":\"Systemtest.Data.Intergration.Api\",\"type\":\"event\"}";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "timeused_aggregation",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertEventTimeUsedSuccess()
    {
        Console.WriteLine(nameof(InsertEventTimeUsedSuccess));
        var json = "{\r\n  \"type\": \"event\",\r\n  \"version\": \"1.0\",\r\n  \"created\": \"2018-08-21T18:25:43.511Z\",\r\n  \"id\": \"05ac9834-5421-44a5-ae81-3596ceff6ca0\",\r\n  \"routing\": {\r\n    \"action\": \"learnapp.user_navigation.complete.course\",\r\n    \"actionVersion\": \"1.2\",\r\n    \"entity\": \"coursepad.course.courseobject\",\r\n    \"entityId\": \"{course-id}\"\r\n  },\r\n  \"payload\": {\r\n    \"identity\": {\r\n      \"clientId\": \"random('232','28','33')\",\r\n      \"customerId\": \"12\",\r\n      \"sourceIp\": \"stringMerge(.,10.0.0,cur.clientId)\",\r\n      \"userId\": \"uuid()\",\r\n      \"onBehalfOfUser\": \"uuid()\"\r\n    },\r\n    \"references\": {\r\n      \"externalId\": \"ui-user-navigation-id-33\",\r\n      \"correlationId\": \"uuid()\",\r\n      \"commandId\": \"uuid()\",\r\n      \"eventId\": \"uuid()\"\r\n    },\r\n    \"body\": {\r\n      \"course_id\": \"random('11','22','33','44','55','66','77','88','99')\",\r\n      \"timeused\": \"4\",\r\n      \"comment\": \"alphaNumeric(30)\",\r\n      \"rating_date\": \"nowTimestamp()\",\r\n      \"reviewer\": \"firstName()\"\r\n    }\r\n  }\r\n}";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "timeused_aggregation",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertSnapShotSuccess()
    {
        Console.WriteLine(nameof(InsertSnapShotSuccess));
        var json = "{\r\n  \"type\": \"event\",\r\n  \"version\": \"1.0\",\r\n  \"created\": \"2018-08-21T18:25:43.511Z\",\r\n  \"id\": \"05ac9834-5421-44a5-ae81-3596ceff6ca0\",\r\n  \"routing\": {\r\n    \"action\": \"learnapp.user_navigation.complete.course\",\r\n    \"actionVersion\": \"1.2\",\r\n    \"entity\": \"coursepad.course.courseobject\",\r\n    \"entityId\": \"{course-id}\"\r\n  },\r\n  \"payload\": {\r\n    \"identity\": {\r\n      \"clientId\": \"random('232','28','33')\",\r\n      \"customerId\": \"12\",\r\n      \"sourceIp\": \"stringMerge(.,10.0.0,cur.clientId)\",\r\n      \"userId\": \"uuid()\",\r\n      \"onBehalfOfUser\": \"uuid()\"\r\n    },\r\n    \"references\": {\r\n      \"externalId\": \"ui-user-navigation-id-33\",\r\n      \"correlationId\": \"uuid()\",\r\n      \"commandId\": \"uuid()\",\r\n      \"eventId\": \"uuid()\"\r\n    },\r\n    \"body\": {\r\n      \"course_id\": \"random('11','22','33','44','55','66','77','88','99')\",\r\n      \"timeused\": \"4\",\r\n      \"comment\": \"alphaNumeric(30)\",\r\n      \"rating_date\": \"nowTimestamp()\",\r\n      \"reviewer\": \"firstName()\"\r\n    }\r\n  }\r\n}";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "snapshot",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertCommandInsertUserSuccess()
    {
        Console.WriteLine(nameof(InsertCommandInsertUserSuccess));
        var json = "{\r\n    \"type\": \"command\",\r\n    \"payload\": {\r\n      \"identity\": {\r\n        \"customerId\": \"37d7a0e0-bb16-4576-9508-d1b636bd51ce\",\r\n        \"userId\": \"f6e021af-a6a0-4039-83f4-152595b4671t\",\r\n        \"clientId\": \"actionstarter\"\r\n      },\r\n      \"references\": {\r\n        \"correlationId\": \"2373975f-6d1e-4349-8a07-521580a95583\",\r\n        \"commandId\": null\r\n      },\r\n      \"body\": {\r\n        \"emailAddress\": \"Persie19Sept201816@example.com\",\r\n        \"orgIds\": null,\r\n        \"username\": \"Persie19Sept2018116\",\r\n        \"countryCode\": \"00084\",\r\n        \"gender\": 2,\r\n        \"dateOfBirth\": \"2018-09-13T03:49:24.121Z\",\r\n        \"accessPlanIds\": null,\r\n        \"firstName\": \"Persie\",\r\n        \"isSendConfirmationEmail\": true,\r\n        \"isActive\": true,\r\n        \"country\": \"VietNam\",\r\n        \"password\": \"\",\r\n        \"clientId\": \"\",\r\n        \"roles\": [],\r\n        \"lastName\": \"Van\",\r\n        \"returnUrl\": \"\",\r\n        \"phoneNumber\": \"938041205\",\r\n        \"lockoutEnabled\": true,\r\n        \"isRequestOTP\": true\r\n      }\r\n    },\r\n    \"@version\": \"1\",\r\n    \"version\": \"1.0\",\r\n    \"routing\": {\r\n      \"entity\": \"cxid.userschema.user\",\r\n      \"entityId\": null,\r\n      \"actionVersion\": \"1.0\",\r\n      \"action\": \"cxid.crud.create.user\"\r\n    },\r\n    \"@timestamp\": \"2018-09-19T11:28:55.775Z\",\r\n    \"id\": \"8b1b6cbe-ee7f-4018-8480-59bcaa406db3\"\r\n  }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "timeused_aggregation",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertUserCommandSuccess()
    {
        Console.WriteLine(nameof(InsertUserCommandSuccess));
        var json = @"{
                      ""type"": ""command"",
                      ""version"": ""1.0"",
                      ""id"": ""08444074-ec6b-4e97-8baa-25500f595823"",
                      ""routing"": {
                                ""action"": ""cxid.crud.create.user"",
                        ""actionVersion"": ""1.0"",
                        ""entity"": ""cxid.userschema.user"",
                        ""entityId"": null
                      },
                      ""payload"": {
                                ""identity"": {
                                    ""clientId"": ""CandidateApi"",
                          ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                          ""userId"": ""f6e021af-a6a0-4039-83f4-152595b4671a"",
                          ""sourceIp"": ""::1""
                                },
                        ""references"": {
                                    ""correlationId"": ""52119a31-9469-469f-babd-96f52268a91c"",
                          ""commandId"": null
                        },
                        ""body"": {
                                    ""username"": ""example12"",
                          ""password"": """",
                          ""firstName"": ""long"",
                          ""lastName"": ""nguyen"",
                          ""emailAddress"": ""example12@gmail.com"",
                          ""isActive"": true,
                          ""clientId"": """",
                          ""returnUrl"": """",
                          ""isSendConfirmationEmail"": false,
                          ""isRequestOTP"": false,
                          ""countryCode"": """",
                          ""gender"": 0,
                          ""dateOfBirth"": ""1986-06-29T00:00:00"",
                          ""phoneNumber"": """",
                          ""phoneNumberConfirmed"": false,
                          ""roles"": [
                            ""User""
                          ],
                          ""orgIds"": null,
                          ""accessPlanIds"": null,
                          ""country"": """",
                          ""lockoutEnabled"": true
                        }
                      }
                    }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.crud.create.user",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertUserEventSuccess()
    {
        Console.WriteLine(nameof(InsertUserEventSuccess));
        var userID = Guid.NewGuid().ToString();
        var email = $"{userID}@example.com";
        var json = @"{
              ""type"": ""event"",
              ""version"": ""1.0"",
              ""id"": ""3ce481bc-c88c-49ed-8e2e-fbf43aa81d21"",
              ""routing"": {
                ""action"": ""cxid.system_success.create.user"",
                ""actionVersion"": ""1.0"",
                ""entity"": ""cxid.orgschema.user"",
                ""entityId"": ""09c19676-a9fc-408e-823f-c591675d5860""
              },
              ""payload"": {
                ""identity"": {
                  ""clientId"": ""CandidateApi"",
                  ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                  ""userId"": """+ userID + @""",
                  ""sourceIp"": ""::1""
                },
                ""references"": {
                  ""correlationId"": ""af7d5009-70a3-42a6-b7e4-a0c2554fc927"",
                  ""commandId"": ""9e10c085-9021-48e4-8d8d-baaa25dfc9f9""
                },
                ""body"": {
                  ""firstName"": ""long"",
                  ""lastName"": ""nguyen"",
                  ""isActive"": true,
                  ""orgs"": [
                    ""Tearc3""
                  ],
                  ""createdDate"": ""2019-01-09T04:43:12.8372164Z"",
                  ""lastLoginDate"": null,
                  ""lastLogoutDate"": null,
                  ""mobileLastLoginDate"": null,
                  ""isAccountInActiveDirectory"": false,
                  ""countryCode"": """",
                  ""gender"": 0,
                  ""dateOfBirth"": ""1986-06-29T00:00:00"",
                  ""saltPassword"": null,
                  ""country"": """",
                  ""modifiedDate"": ""2019-01-09T04:43:12.8372164Z"",
                  ""usedPasswordHashs"": null,
                  ""passwordModifiedDate"": null,
                  ""otp"": null,
                  ""otpExpiration"": null,
                  ""id"": """+ userID + @""",
                  ""userName"": """+ userID + @""",
                  ""normalizedUserName"": """+ userID + @""",
                  ""email"": """+ email + @""",
                  ""normalizedEmail"": """+ email + @""",
                  ""emailConfirmed"": true,
                  ""passwordHash"": null,
                  ""securityStamp"": ""321757b4-3286-4449-8ff9-8be10a52b955"",
                  ""concurrencyStamp"": ""6b3a29de-c8e4-4b69-b6e0-1b980da79d35"",
                  ""phoneNumber"": """",
                  ""phoneNumberConfirmed"": false,
                  ""twoFactorEnabled"": false,
                  ""lockoutEnd"": null,
                  ""lockoutEnabled"": true,
                  ""accessFailedCount"": 0
                }
              }
            }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.system_success.create.user",
                             basicProperties: null,
                             body: body);
    }

    private static void UpdateUserEventSuccess()
    {
        Console.WriteLine(nameof(UpdateUserEventSuccess));
        var json = @"{
              ""type"": ""event"",
              ""version"": ""1.0"",
              ""id"": ""3ce481bc-c88c-49ed-8e2e-fbf43aa81d21"",
              ""routing"": {
                ""action"": ""cxid.system_success.update.user"",
                ""actionVersion"": ""1.0"",
                ""entity"": ""cxid.orgschema.user"",
                ""entityId"": ""5da68b9b-6526-4495-9edd-39293fe0c196""
              },
              ""payload"": {
                ""identity"": {
                  ""clientId"": ""CandidateApi"",
                  ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                  ""userId"": ""f6e021af-a6a0-4039-83f4-152595b4671a"",
                  ""sourceIp"": ""::1""
                },
                ""references"": {
                  ""correlationId"": ""af7d5009-70a3-42a6-b7e4-a0c2554fc927"",
                  ""commandId"": ""9e10c085-9021-48e4-8d8d-baaa25dfc9f9""
                },
                ""body"": {
                  ""firstName"": ""updatedlong"",
                  ""lastName"": ""updatednguyen"",
                  ""isActive"": false,
                  ""createdDate"": ""2019-01-09T04:43:12.8372164Z"",
                  ""lastLoginDate"": null,
                  ""lastLogoutDate"": null,
                  ""mobileLastLoginDate"": null,
                  ""isAccountInActiveDirectory"": false,
                  ""countryCode"": ""12345"",
                  ""gender"": 1,
                  ""dateOfBirth"": ""1987-06-29T00:00:00"",
                  ""saltPassword"": null,
                  ""country"": """",
                  ""modifiedDate"": ""2019-01-09T04:43:12.8372164Z"",
                  ""usedPasswordHashs"": null,
                  ""passwordModifiedDate"": null,
                  ""otp"": null,
                  ""otpExpiration"": null,
                  ""id"": ""5da68b9b-6526-4495-9edd-39293fe0c196"",
                  ""userName"": ""example11"",
                  ""normalizedUserName"": ""EXAMPLE11"",
                  ""email"": ""example11@gmail.com"",
                  ""normalizedEmail"": ""EXAMPLE11@GMAIL.COM"",
                  ""emailConfirmed"": true,
                  ""passwordHash"": null,
                  ""securityStamp"": ""321757b4-3286-4449-8ff9-8be10a52b955"",
                  ""concurrencyStamp"": ""6b3a29de-c8e4-4b69-b6e0-1b980da79d35"",
                  ""phoneNumber"": """",
                  ""phoneNumberConfirmed"": false,
                  ""twoFactorEnabled"": false,
                  ""lockoutEnd"": null,
                  ""lockoutEnabled"": true,
                  ""accessFailedCount"": 0
                }
              }
            }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.system_success.update.user",
                             basicProperties: null,
                             body: body);
    }

    private static void DeleteUserEventSuccess()
    {
        Console.WriteLine(nameof(DeleteUserEventSuccess));
        var json = @"{
              ""type"": ""event"",
              ""version"": ""1.0"",
              ""id"": ""3ce481bc-c88c-49ed-8e2e-fbf43aa81d21"",
              ""routing"": {
                ""action"": ""cxid.system_success.delete.user"",
                ""actionVersion"": ""1.0"",
                ""entity"": ""cxid.orgschema.user"",
                ""entityId"": ""5da68b9b-6526-4495-9edd-39293fe0c196""
              },
              ""payload"": {
                ""identity"": {
                  ""clientId"": ""CandidateApi"",
                  ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                  ""userId"": ""f6e021af-a6a0-4039-83f4-152595b4671a"",
                  ""sourceIp"": ""::1""
                },
                ""references"": {
                  ""correlationId"": ""af7d5009-70a3-42a6-b7e4-a0c2554fc927"",
                  ""commandId"": ""9e10c085-9021-48e4-8d8d-baaa25dfc9f9""
                },
                ""body"": {
                  ""firstName"": ""updatedlong"",
                  ""lastName"": ""updatednguyen"",
                  ""isActive"": true,
                  ""createdDate"": ""2019-01-09T04:43:12.8372164Z"",
                  ""lastLoginDate"": null,
                  ""lastLogoutDate"": null,
                  ""mobileLastLoginDate"": null,
                  ""isAccountInActiveDirectory"": false,
                  ""countryCode"": ""12345"",
                  ""gender"": 0,
                  ""dateOfBirth"": ""1986-06-29T00:00:00"",
                  ""saltPassword"": null,
                  ""country"": """",
                  ""modifiedDate"": ""2019-01-09T04:43:12.8372164Z"",
                  ""usedPasswordHashs"": null,
                  ""passwordModifiedDate"": null,
                  ""otp"": null,
                  ""otpExpiration"": null,
                  ""id"": ""5da68b9b-6526-4495-9edd-39293fe0c196"",
                  ""userName"": ""example11"",
                  ""normalizedUserName"": ""EXAMPLE11"",
                  ""email"": ""example11@gmail.com"",
                  ""normalizedEmail"": ""EXAMPLE11@GMAIL.COM"",
                  ""emailConfirmed"": true,
                  ""passwordHash"": null,
                  ""securityStamp"": ""321757b4-3286-4449-8ff9-8be10a52b955"",
                  ""concurrencyStamp"": ""6b3a29de-c8e4-4b69-b6e0-1b980da79d35"",
                  ""phoneNumber"": """",
                  ""phoneNumberConfirmed"": false,
                  ""twoFactorEnabled"": false,
                  ""lockoutEnd"": null,
                  ""lockoutEnabled"": true,
                  ""accessFailedCount"": 0
                }
              }
            }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.system_success.delete.user",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertOrgSuccess()
    {
        Console.WriteLine(nameof(InsertOrgSuccess));
        var json = @"{
              ""type"": ""event"",
              ""version"": ""1.0"",
              ""id"": ""961a8f78-42a2-4f84-8518-f440998c659b"",
              ""routing"": {
                ""action"": ""cxid.system_success.create.organizationalunit"",
                ""actionVersion"": ""1.0"",
                ""entity"": ""cxid.userschema.organizationalunit"",
                ""entityId"": ""0e89015e-f3fd-43fd-bae1-45b88b1c4a73""
              },
              ""payload"": {
                    ""identity"": {
                        ""clientId"": ""CandidateApi"",
                        ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                        ""userId"": ""f6e021af-a6a0-4039-83f4-152595b4671a"",
                        ""sourceIp"": ""::1""
                    },
                    ""references"": {
                        ""correlationId"": ""dbe34209-55ec-4cad-a031-7eb31f7049e6"",
                        ""commandId"": ""be0ce2f3-955a-4fe2-ac72-38cf2f5b5f3a""
                    },
                    ""body"": {
                        ""id"": ""0e89015e-f3fd-43fd-bae1-45b88b1c4a73"",
                        ""name"": ""Tearc3"",
                        ""location"": ""vietnam"",
                        ""timezone"": ""Asia/Hanoi"",
                        ""locale"": ""vn"",
                        ""currency"": ""VND"",
                        ""createdDate"": ""2019-01-17T06:52:01.5023383Z"",
                        ""modifiedDate"": ""2019-01-17T06:52:01.5023383Z"",
                        ""additionalProperties"": null,
                        ""organizationalUnitUsers"": null
                    }
                }
            }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.system_success.create.organizationalunit",
                             basicProperties: null,
                             body: body);
    }

    private static void InsertOrgUnitUserSuccess()
    {
        Console.WriteLine(nameof(InsertOrgUnitUserSuccess));
        var json = @"{
                  ""type"": ""event"",
                  ""version"": ""1.0"",
                  ""id"": ""e3c2f0d3-5aca-43ed-a3ca-a267dc53b78e"",
                  ""routing"": {
                    ""action"": ""cxid.system_success.create.organizationalunituser"",
                    ""actionVersion"": ""1.0"",
                    ""entity"": ""cxid.userschema.user"",
                    ""entityId"": ""5da68b9b-6526-4495-9edd-39293fe0c196""
                  },
                  ""payload"": {
                        ""identity"": {
                        ""clientId"": ""CandidateApi"",
                        ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                        ""userId"": ""f6e021af-a6a0-4039-83f4-152595b4671a"",
                        ""sourceIp"": ""::1""
                        },
                        ""references"": {
                            ""correlationId"": ""5ae0d861-7d0b-4042-9d98-fe054ecf06a7"",
                            ""commandId"": ""26fe7827-de90-4970-a4c9-630d6df1ebde""
                        },
                        ""body"": {
                            ""organizationalUnitId"": ""0e89015e-f3fd-43fd-bae1-45b88b1c4a73"",
                            ""organizationalUnit"": null,
                            ""userId"": ""5da68b9b-6526-4495-9edd-39293fe0c196""
                        }
                    }
                }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.system_success.create.organizationalunituser",
                             basicProperties: null,
                             body: body);
    }

    private static void DeleteOrgUnitUserSuccess()
    {
        Console.WriteLine(nameof(DeleteOrgUnitUserSuccess));
        var json = @"{
                      ""type"": ""event"",
                      ""version"": ""1.0"",
                      ""id"": ""d5a764cd-ecf5-41ec-b28f-80f9bc962161"",
                      ""routing"": {
                        ""action"": ""cxid.system_success.delete.organizationalunituser"",
                        ""actionVersion"": ""1.0"",
                        ""entity"": ""cxid.userschema.user"",
                        ""entityId"": ""f6e021af-a6a0-4039-83f4-152595b4671a""
                      },
                      ""payload"": {
                            ""identity"": {
                            ""clientId"": ""CandidateApi"",
                            ""customerId"": ""37d7a0e0-bb16-4576-9508-d1b636bd51ce"",
                            ""userId"": ""f6e021af-a6a0-4039-83f4-152595b4671a"",
                            ""sourceIp"": ""::1""
                        },
                        ""references"": {
                            ""correlationId"": ""600fef20-16e3-4b8e-b1b0-7d79818db539"",
                            ""commandId"": ""57aefb64-8ef2-4552-9c65-eb5a65eab615""
                        },
                        ""body"": {
                            ""organizationalUnitId"": ""0e89015e-f3fd-43fd-bae1-45b88b1c4a73"",
                            ""organizationalUnit"": {
                                ""id"": ""0e89015e-f3fd-43fd-bae1-45b88b1c4a73"",
                                ""name"": ""CXS International [Test]"",
                                ""location"": ""Lot B-05-01, Tamarind Square Persiaran Multimedia, Cyber 11, Cyberjaya, Selangor"",
                                ""timezone"": ""Asia/Kuala_Lumpur"",
                                ""locale"": ""en"",
                                ""currency"": ""USD"",
                                ""createdDate"": ""2019-01-17T15:20:45.5457968"",
                                ""modifiedDate"": ""2019-01-17T15:20:45.5457968"",
                                ""additionalProperties"": null,
                                ""organizationalUnitUsers"": []
                            },
                            ""userId"": ""5da68b9b-6526-4495-9edd-39293fe0c196""
                        }
                      }
                    }";
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "event_exchange_topic",
                             routingKey: "cxid.system_success.delete.organizationalunituser",
                             basicProperties: null,
                             body: body);
    }
}
