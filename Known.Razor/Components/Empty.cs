﻿namespace Known.Razor.Components;

public class Empty : BaseComponent
{
    [Parameter] public string Icon { get; set; } = "fa fa-commenting-o";
    [Parameter] public string Text { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("empty", attr =>
        {
            builder.Icon(Icon);
            builder.Paragraph(attr => builder.Text(Text));
        });
    }
}