﻿namespace Known.Razor.Components.Fields;

public class Captcha : Field
{
    private const string Chars = "abcdefghijkmnpqrstuvwxyz2345678ABCDEFGHJKLMNPQRSTUVWXYZ";
    private readonly string id;

    public Captcha()
    {
        id = Utils.GetGuid();
        CreateCode();
    }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    public string Code { get; set; }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        UI.Captcha(id, Code);
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
                .Placeholder(Placeholder)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder())
                .OnEnter(OnEnter);
        });
        BuildImage(builder);
    }

    private static void BuildIcon(RenderTreeBuilder builder, string icon)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
    }

    private void BuildImage(RenderTreeBuilder builder)
    {
        builder.Element("canvas", attr =>
        {
            attr.Id(id).Class("captcha").Title("点击图片刷新").OnClick(Callback(e => CreateCode()));
        });
    }

    private void CreateCode()
    {
        var rnd = new Random();
        Code = "";
        for (int i = 0; i < 4; i++)
        {
            Code += Chars[rnd.Next(Chars.Length)];
        }
    }
}