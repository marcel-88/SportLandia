using AutoMapper;
using eUseControl.Domain.Entities.User;

namespace eUseControl.BusinessLogic.AutoMapper
{
public class AutoMapperConfig
{
  public static MapperConfiguration ConfigureMappings()
  {
    var config = new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<ULoginData, UserMinimal>();
      // Add more mappings if needed
    });
    return config;
  }
}
}
