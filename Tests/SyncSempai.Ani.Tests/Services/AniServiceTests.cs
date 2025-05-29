using Microsoft.Extensions.Logging;
using Moq;
using SyncSempai.Ani.Services;

namespace SyncSempai.Ani.Tests.Services
{
    public class AniServiceTests
    {
        [Fact]
        public async Task GetUserWatchListAsync_ReturnsExpected()
        {
            using (var client = new HttpClient())
            {
                ILogger<AniService> logger = Mock.Of<ILogger<AniService>>();

                var aniservice = new AniService(logger, client);

                var result = await aniservice.GetUserWatchListAsync("TestUser");

                Assert.NotNull(result.Data);
            }
        }
    }
}