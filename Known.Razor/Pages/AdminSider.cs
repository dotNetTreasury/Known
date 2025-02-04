﻿namespace Known.Razor.Pages;

class AdminSider : BaseComponent
{
    [Parameter] public MenuItem CurMenu { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-scroll", attr => BuildMenuTree(builder));
    }

    private void BuildMenuTree(RenderTreeBuilder builder)
    {
        builder.Component<Menu>()
               .Set(c => c.Style, "menu-tree")
               .Set(c => c.TextIcon, true)
               .Set(c => c.Items, Menus)
               .Set(c => c.OnClick, OnNavItemClick)
               .Build();
    }

    private void OnNavItemClick(MenuItem item) => Context.Navigate(item);
}