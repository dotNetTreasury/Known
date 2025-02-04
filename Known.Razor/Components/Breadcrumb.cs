﻿namespace Known.Razor.Components;

public class Breadcrumb : BaseComponent
{
    [Parameter] public MenuItem Menu { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Ul("breadcrumb", attr =>
        {
            if (Menu != null)
            {
                BuildHome(builder);
                BuildItem(builder, Menu, false);
            }
            else if (Items != null && Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    BuildItem(builder, item);
                }
            }
        });
    }

    private void BuildHome(RenderTreeBuilder builder)
    {
        builder.Li("home", attr =>
        {
            attr.OnClick(Callback(e => Context.NavigateToHome()));
            builder.IconName("fa fa-home", "首页");
        });
    }

    private void BuildItem(RenderTreeBuilder builder, MenuItem item, bool showIcon = true)
    {
        if (item.Parent != null)
            BuildItem(builder, item.Parent, showIcon);

        builder.Li(attr =>
        {
            if (item.Action != null)
                attr.OnClick(Callback(item.Action));
            if (showIcon)
                builder.IconName(item.Icon, item.Name);
            else
                builder.Span(item.Name);
        });
    }
}