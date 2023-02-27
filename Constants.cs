using SQLite;
public static class Constants
{
    public const string DatabaseFilename = "TrackingMarques.db3";

    public const SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public static string RutaFitxer = "/storage/emulated/0/Documents/";
    public static string ExtensioFitxer = "xml";
    public static string TextPuntsInteres = "Punts interés introduïts";
    public static string TextPuntsRuta = "Punts ruta introduïts";
}