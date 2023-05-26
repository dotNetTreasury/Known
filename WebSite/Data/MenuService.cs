namespace WebSite.Data;

class MenuService
{
    internal static List<MenuItem> GetDocMenus()
    {
        return new List<MenuItem>
        {
            new MenuItem("����")
            {
                Children = new List<MenuItem>
                {
                    new MenuItem("���", "/doc/profile"),
                    new MenuItem("���ٿ�ʼ", "/doc"),
                }
            }
        };
    }
}