namespace WebApp1.Jobs;

public class JobSettings
{
    public int ImportEventsMinutePeriod { get; set; }
    public int ImportClientsMinutePeriod { get; set; }
    public int ImportTicketsMinutePeriod { get; set; }

    public TimeSpan ImportEventsPeriod => TimeSpan.FromMinutes(ImportEventsMinutePeriod);
    public TimeSpan ImportClientsPeriod => TimeSpan.FromMinutes(ImportClientsMinutePeriod);
    public TimeSpan ImportTicketsPeriod => TimeSpan.FromMinutes(ImportTicketsMinutePeriod);
}