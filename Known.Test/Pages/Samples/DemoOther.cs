﻿namespace Known.Test.Pages.Samples;

class DemoOther : BaseComponent
{
    private readonly List<MenuItem> tabItems = new()
    {
        new MenuItem{Icon="fa fa-file-o",Name="Tab1"},
        new MenuItem{Icon="fa fa-file-o",Name="Tab2"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBreadcrumb(builder);
        builder.Div("row", attr =>
        {
            BuildDemo(builder, "时间", () => builder.Component<Razor.Components.Timer>().Build());
            BuildDemo(builder, "搜索框", () => builder.Component<SearchBox>().Build());
            BuildDemo(builder, "验证码", () => builder.Component<Captcha>().Build());
        });
        BuildBanner(builder);
        BuildNotify(builder);
        BuildToast(builder);
        BuildCarousel(builder);
        BuildCard(builder);
        BuildTabs(builder);
    }

    private void BuildBanner(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "横幅通知", () =>
        {
            builder.Component<Banner>().Set(c => c.Content, b => b.Span("bold", "这里是默认横幅通知！")).Build();
            builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是主要横幅通知！")).Set(c => c.Style, StyleType.Primary).Build();
            builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是成功横幅通知！")).Set(c => c.Style, StyleType.Success).Build();
            builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是信息横幅通知！")).Set(c => c.Style, StyleType.Info).Build();
            builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是警告横幅通知！")).Set(c => c.Style, StyleType.Warning).Build();
            builder.Component<Banner>().Set(c => c.Content, b => b.Text("这里是危险横幅通知！")).Set(c => c.Style, StyleType.Danger).Build();
        });
    }

    private void BuildNotify(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "通知", () =>
        {
            builder.Button("默认", Callback(() => UI.Notify("<h1>这里是默认通知！</h1>")));
            builder.Button("主要", Callback(() => UI.Notify("这里是主要通知！", StyleType.Primary, 10000)), StyleType.Primary);
            builder.Button("成功", Callback(() => UI.Notify("这里是成功通知！", StyleType.Success)), StyleType.Success);
            builder.Button("信息", Callback(() => UI.Notify("这里是信息通知！", StyleType.Info)), StyleType.Info);
            builder.Button("警告", Callback(() => UI.Notify("这里是警告通知！", StyleType.Warning)), StyleType.Warning);
            builder.Button("危险", Callback(() => UI.Notify("这里是危险通知！", StyleType.Danger)), StyleType.Danger);
        });
    }

    private void BuildToast(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "提示", () =>
        {
            builder.Button("默认", Callback(() => UI.Toast("这里是默认提示！")));
            builder.Button("主要", Callback(() => UI.Toast("这里是主要提示！", StyleType.Primary)), StyleType.Primary);
            builder.Button("成功", Callback(() => UI.Toast("这里是成功提示！", StyleType.Success)), StyleType.Success);
            builder.Button("信息", Callback(() => UI.Toast("这里是信息提示！", StyleType.Info)), StyleType.Info);
            builder.Button("警告", Callback(() => UI.Toast("这里是警告提示！", StyleType.Warning)), StyleType.Warning);
            builder.Button("危险", Callback(() => UI.Toast("这里是危险提示！", StyleType.Danger)), StyleType.Danger);
        });
    }

    private void BuildBreadcrumb(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "面包屑", () =>
        {
            builder.Component<Breadcrumb>().Set(c => c.Items, new List<MenuItem>
            {
                new MenuItem("Test1", "测试1", "fa fa-home"),
                new MenuItem("Test2", "测试2", "fa fa-user"),
                new MenuItem("Test3", "测试3") {Action=()=>UI.Alert("Test")},
                new MenuItem("Test4", "测试4")
            }).Build();
        });
    }

    private static void BuildCarousel(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "走马灯", () =>
        {
            builder.Div("box", attr =>
            {
                attr.Style("width:50%;height:100px;");
                builder.Component<Carousel>().Build();
            });
        });
    }

    private static void BuildCard(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "卡片", () =>
        {
            builder.Div("row", attr =>
            {
                builder.Div("box", attr =>
                {
                    builder.Component<Card>().Set(c => c.Name, "Card1").Build();
                });
                builder.Div("box", attr =>
                {
                    builder.Component<Card>().Set(c => c.Icon, "fa fa-list").Set(c => c.Name, "Card2").Build();
                });
            });
        });
    }

    private void BuildTabs(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "选项卡", () =>
        {
            builder.Div("row", attr =>
            {
                builder.Div("box", attr =>
                {
                    builder.Component<Tabs>()
                           .Set(c => c.CurItem, tabItems[0])
                           .Set(c => c.Items, tabItems)
                           .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                           .Build();
                });
                builder.Div("box", attr =>
                {
                    builder.Component<Tabs>()
                           .Set(c => c.Justified, true)
                           .Set(c => c.CurItem, tabItems[0])
                           .Set(c => c.Items, tabItems)
                           .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                           .Build();
                });
                builder.Div("box", attr =>
                {
                    builder.Component<Tabs>()
                           .Set(c => c.CurItem, tabItems[0])
                           .Set(c => c.Items, tabItems)
                           .Set(c => c.Position, PositionType.Left)
                           .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                           .Build();
                });
                builder.Div("box", attr =>
                {
                    builder.Component<Tabs>()
                           .Set(c => c.CurItem, tabItems[0])
                           .Set(c => c.Items, tabItems)
                           .Set(c => c.Position, PositionType.Bottom)
                           .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                           .Build();
                });
            });
        });
    }

    private static void BuildDemo(RenderTreeBuilder builder, string text, Action action)
    {
        builder.Div("demo-row", attr =>
        {
            builder.Div("demo-caption", text);
            action();
        });
    }
}