using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using cxOrganization.Client.Customers;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Services;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cxOrganization.WebServiceAPI.Controllers
{
    /// <summary>
    /// Customer API(do not require cxtoken)
    /// </summary>
    [Authorize]
    public class CustomerController : ApiControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAdvancedWorkContext _workContext;
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="workContext"></param>
        public CustomerController(ICustomerService customerService, IAdvancedWorkContext workContext)
        {
            _customerService = customerService;
            _workContext = workContext;
        }

        /// <summary>
        /// Get customers
        /// </summary>
        /// <param name="ownerid"></param>
        /// <param name="customerIds"></param>
        /// <param name="extIds"></param>
        /// <param name="createdBefore"></param>
        /// <param name="createdAfter"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [Route("owners/{ownerid}/customers")]
        [HttpGet]
        public IActionResult GetCustomersByOwner(int ownerid,
            [FromQuery] List<int> customerIds = null,
            [FromQuery] List<string> extIds = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null)
        {
            var customerDtos = _customerService.GetCustomers(customerids: customerIds,
                ownerids: new List<int>() {ownerid},
                extids: extIds,
                createdBefore: createdBefore,
                createdAfter: createdAfter);
            return CreateResponse<CustomerDto>(customerDtos);           
        }

        /// <summary>
        /// Insert customer
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [Route("owners/{ownerid}/customers")]
        [HttpPost]
        public IActionResult InsertCustomer([FromBody]CustomerDto customer, int ownerId)
        {
            customer.Identity.Id = 0;
            customer.Identity.OwnerId = ownerId;
            return StatusCode((int)System.Net.HttpStatusCode.Created,_customerService.InsertCustomer(customer));
        }
        /// <summary>
        /// Get customer
        /// </summary>
        /// <param name="ownerid"></param>
        /// <param name="customerid"></param>
        /// <returns></returns>
        [Route("owners/{ownerid}/customers/{customerid}")]
        [HttpGet]
        public IActionResult GetCustomersByOwner(int ownerid,
            int customerid)
        {
            var customerDto = _customerService.GetCustomers(customerids: new List<int> { customerid },
                ownerids: new List<int>() { ownerid }).FirstOrDefault();
            return CreateResponse<CustomerDto>(customerDto);
        }
        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="customerId"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [Route("owners/{ownerid}/customers/{customerid}")]
        [HttpPut]
        public IActionResult UpdateCustomer([FromBody]CustomerDto customer, int customerId, int ownerId)
        {
            customer.Identity.Id = customerId;
            customer.Identity.OwnerId = ownerId;
            return CreateResponse(_customerService.UpdateCustomer(customer));
        }

    }
}