using Moq;
using WebApp1.External.Qtickets;
namespace TestProject;


public class Tests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task Test1()
    {
        const string token = "oKwXDV5zjGB4PpfPf0JCC0zc0wLPhH6c";
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://qtickets.ru/api/rest/v1/")
        };
        httpClientFactoryMock.Setup(x => x.CreateClient("Qtickets")).Returns(httpClient);
        var qticketsApiProvider = new QticketsApiProvider(httpClientFactoryMock.Object);
        await qticketsApiProvider.GetClients(token).ToListAsync();
        
    }
}