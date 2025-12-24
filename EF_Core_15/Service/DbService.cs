using EF_Core_15.Models;

namespace EF_Core_15.Service;

public class DbService
{
    private TestContext _context;
    public TestContext Context=>_context;

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
        _context = new TestContext();
    }
}