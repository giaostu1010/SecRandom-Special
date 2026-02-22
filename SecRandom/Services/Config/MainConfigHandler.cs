using Microsoft.Extensions.Logging;
using SecRandom.Core.Abstraction;
using SecRandom.Models;

namespace SecRandom.Services.Config;

public class MainConfigHandler(ILogger<MainConfigHandler> logger, ConfigServiceBase configService)
    : ConfigHandlerBase<MainConfigModel>(logger, configService, () => new MainConfigModel());