using Microsoft.AspNetCore.Components;
using WebSite.Core;

namespace WebSite.Data;

class DocService
{
    internal static List<MenuItem> GetDocMenus()
    {
        return new List<MenuItem>
        {
            new MenuItem("overview", "����")
            {
                Children = new List<MenuItem>
                {
                    new MenuItem("profile", "���"),
                    new MenuItem("start", "���ٿ�ʼ"),
                }
            }
        };
    }

    internal static MarkupString GetDocHtml(string? id)
    {
        if (id == "profile")
        {
            var markdown = new Markdown();
            var text = File.ReadAllText(@"D:\Publics\Known\README.md");
            var html = markdown.Transform(text);
            return new MarkupString(html);
        }

        return new MarkupString(id);
    }
}