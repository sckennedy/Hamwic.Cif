namespace Hamwic.Cif.Core.Data
{
    public static class DbInitialiser
    {
        public static void Initialise(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}