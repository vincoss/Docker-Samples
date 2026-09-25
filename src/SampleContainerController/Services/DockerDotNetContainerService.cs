using SampleContainerController.Dto;
using SampleContainerController.Interface;
using System;
using System.Collections.Generic;
using System.Text;


namespace SampleContainerController.Services
{
    public class DockerDotNetContainerService : IAppContainerService
    {
        public Task StartAsync(ContainerJobDto job, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
