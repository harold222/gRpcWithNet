using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrasactionServer;

namespace BlockChain.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        private readonly ILogger<CustomerService> _logger;
        public CustomerService(ILogger<CustomerService> logger)
        {
            _logger = logger;
        }

        public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModel request, ServerCallContext context)
        {
            return Task.FromResult(new CustomerModel
            {
                Age = 18,
                FirstName = "test",
                EmailAddress = "test@test.co",
                IsAlive = true,
                LastName = "other"
            });
        }

        public override async Task GetNewCustomers(
            NewCustomerRequest request,
            IServerStreamWriter<CustomerModel> responseStream,
            ServerCallContext context
        )
        {
            List<CustomerModel> customers = new List<CustomerModel>()
            {
                new CustomerModel
                {
                    Age = 18,
                    FirstName = "test",
                    EmailAddress = "test@test.co",
                    IsAlive = true,
                    LastName = "other"
                }
            };

            foreach (CustomerModel cust in customers)
            {
                await responseStream.WriteAsync(cust);
            }
        }
    }
}
