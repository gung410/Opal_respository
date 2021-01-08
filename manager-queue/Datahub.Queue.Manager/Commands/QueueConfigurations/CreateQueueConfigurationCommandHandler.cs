using Datahub.Queue.Manager.Data;
using Datahub.Queue.Manager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datahub.Queue.Manager.Domains;

namespace Datahub.Queue.Manager.Commands.QueueConfigurations
{
    public class CreateQueueConfigurationCommandHandler
    {
        private readonly IMongoRepository<QueueConfiguration> _mongoRepository;
        private readonly IMappingService _mappingService;

        public CreateQueueConfigurationCommandHandler(IMongoRepository<QueueConfiguration> mongoRepository,
            IMappingService mappingService)
        {
            _mongoRepository = mongoRepository;
            _mappingService = mappingService;
        }
        public async Task ExecuteAsync(CreateQueueConfigurationCommand command)
        {
            var dataPoint = _mappingService.ToQueueConfiguration(command);
            await _mongoRepository.InsertAsync(dataPoint);
        }
    }
}
