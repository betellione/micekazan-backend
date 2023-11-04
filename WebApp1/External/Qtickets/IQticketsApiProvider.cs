namespace WebApp1.External.Qtickets;

public interface IQticketsApiProvider
{
    public Task GetEvents();
}