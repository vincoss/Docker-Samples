using SampleAzureContainerController.Dto;
using SampleAzureContainerController.Interface;
using System;
using System.Collections.Generic;
using System.Text;


namespace SampleAzureContainerController.Services
{
    public class DockerDotNetContainerService : IAppContainerService
    {
        public Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
