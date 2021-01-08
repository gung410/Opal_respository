using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [ApiController]
    public class ApiControllerBase : Controller
    {
        /// <summary>
        /// Creates the json response for a given entity/DTO object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected IActionResult CreateResponse<T>(List<T> entity)
        {
            if ((entity == null) || (entity.Count == 0))
                return CreateNoContentResponse();
            else
            {
                return Ok(entity);
            }
        }

        /// <summary>
        /// Creates the json response for a given entity/DTO object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="httpStatusCode"></param>
        /// <returns></returns>
        protected IActionResult CreateResponse<T>(HttpStatusCode httpStatusCode, List<T> entity)
        {
            if ((entity == null) || (entity.Count == 0))
                return CreateNoContentResponse();
            else
            {
                return StatusCode((int)httpStatusCode, entity);
            }
        }

        /// <summary>
        /// Creates the json response for a given entity/DTO object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected IActionResult CreateResponse<T>(T entity)
        {
            if (entity == null)
                return CreateNotFoundResponse<T>();
            else
            {
                return Ok(entity);
            }
        }

        /// <summary>
        /// Creates response NoContent: 204
        /// This response is used for getting a list object or PUT/POST which is nothing changed.
        /// </summary>
        /// <returns></returns>
        protected IActionResult CreateNoContentResponse()
        {
            return NoContent();
        }

        /// <summary>
        /// Creates response NotFound: 404
        /// This response is used for getting an object which is not found.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected IActionResult CreateNotFoundResponse(string message)
        {
            return NotFound();
        }

        /// <summary>
        /// Creates response NotFound: 404
        /// This response is used for getting an object which is not found. This is generic method, so the name of object will get dynamically
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IActionResult CreateNotFoundResponse<T>()
        {
            return NotFound();
        }

        /// <summary>
        /// Creates response NoContent: 204
        /// This response is used for getting a list object or PUT/POST which is nothing changed. This is generic method, so the name of object will get dynamically
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IActionResult CreateNoContentResponse<T>()
        {
            return NoContent();
        }

        private string GetTypeName<T>()
        {
            string type = typeof(T).ToString();
            var dtoName = type.Split('.').Last();
            int index = dtoName.IndexOf("Dto");
            return dtoName.Remove(index);
        }

        /// <summary>
        /// Create OK response
        /// </summary>
        /// <returns></returns>
        protected IActionResult CreateSuccessResponse()
        {
            return Ok("OK");
        }

        /// <summary>
        /// Create response with passing HttpStatusCode and Message
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected IActionResult CreateResponse(HttpStatusCode httpStatusCode, string message)
        {
            return StatusCode((int)httpStatusCode, message);
        }

        /// <summary>
        /// Create response BadRequest: 400
        /// This is used for validation cases
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected IActionResult CreateBadRequestResponse(string message)
        {
            return BadRequest(message);
        }

        /// <summary>
        /// Create response BadRequest: 400
        /// This is used for the payload invalid
        /// </summary>
        /// <returns></returns>
        protected IActionResult CreatePayloadExpectedResponse()
        {
            return BadRequest("JSON payload expected");
        }
        

        protected bool ValidBase64Decode(string base64EncodedData, out string data, out IActionResult errorResponseMessage)
        {
            data = null;
            errorResponseMessage = null;
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                data = Encoding.UTF8.GetString(base64EncodedBytes);
                return true;
            }
            catch (Exception)
            {
                errorResponseMessage = CreateBadRequestResponse("Base64 encoded unvalid");
                return false;
            }
        }

        protected IActionResult CreateIsValidModelResponse(ModelStateDictionary modelState)
        {
            var message = string.Join(" | ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(message);
        }

        /// <summary>
        /// Create paging response
        /// The return paging information in HTTP response headers:
        /// X-Paging-PageIndex
        /// X-Paging-PageSize
        /// X-Paging-HasMoreData
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="hasMoreData"></param>
        /// <returns></returns>
        protected IActionResult CreatePagingResponse<T>(List<T> entity, int pageIndex, int pageSize, bool hasMoreData, bool selectIdentity = false)
        {
            if ((entity == null) || (entity.Count == 0))
                return CreateNoContentResponse();
            else
            {
                Response.Headers.Add("X-Paging-PageIndex", pageIndex.ToString());
                Response.Headers.Add("X-Paging-PageSize", pageSize.ToString());
                Response.Headers.Add("X-Paging-HasMoreData", hasMoreData.ToString());
                if (selectIdentity)
                {
                    var selectLinq = BuildExpressionSelectOnlyFieldsNeed<T>("Identity");
                    if (selectLinq != null)
                    {
                        var results = entity.Select(selectLinq).ToList();
                        return Ok(results);
                    }
                }
                return Ok(entity);
            }
        }
        /// <summary>
        /// Build LINQ expression to only select the fields needed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected Func<T, T> BuildExpressionSelectOnlyFieldsNeed<T>(string fields)
        {
            var xParameter = Expression.Parameter(typeof(T), "o");
            var xNew = Expression.New(typeof(T));
            var bindings = fields.Split(',').Select(o => o.Trim()).Where(t => typeof(T).GetProperty(t) != null)
                .Select(o => {
                    var property = typeof(T).GetProperty(o);
                    var xOriginal = Expression.Property(xParameter, property);
                    return Expression.Bind(property, xOriginal);
                }
            );
            if (!bindings.Any())
                return null;
            var xInit = Expression.MemberInit(xNew, bindings);
            var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);
            return lambda.Compile();
        }

        protected IActionResult AccessDenied(string message)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = message });
        }

        protected IActionResult AccessDenied()
        {
            return AccessDenied("Access denied.");
        }
    }
}
