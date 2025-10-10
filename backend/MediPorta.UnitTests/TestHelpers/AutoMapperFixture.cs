using AutoMapper;
using MediPorta.Application.Mappings;

namespace MediPorta.UnitTests.TestHelpers
{
    public static class AutoMapperFixture
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TagProfile>();

            });

            config.AssertConfigurationIsValid();

            return config.CreateMapper();
        }
    }
}
