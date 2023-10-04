namespace Brimborium.Spaetzle;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.HOST.json", optional: true, reloadOnChange: true);
        builder.AddSpaetzle();
        WebApplication app = builder.Build();

        app.UseSpaetzle();
        app.Run();
    }
}