using EF_Core_15.Models;

namespace EF_Core_15.Service;

public class DbService
{
    private Ef15Context _context;
    public Ef15Context Context =>_context;

    private static DbService? instance;

    public static DbService Instance
    {
        get
        {
            if (instance == null)
                instance = new DbService();
            return instance;
        }
    }
    
    public DbService()
    {
        _context = new Ef15Context();
    }
}