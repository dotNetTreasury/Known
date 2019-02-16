﻿//Menu
//{ id, text, parentId?, href?, hrefTarget?, icon, iconCls, cls, expanded, children }

var Menu_Id = 1;

var Menu = function (element, options) {
    this.element = $(element);
    this.options = $.extend(true, {}, this.options, options);
    this.init();
};

Menu.prototype = {

    options: {
        data: null,
        itemclick: null
    },

    loadData: function (data) {
        this.options.data = data || [];
        this.refresh();
    },

    refresh: function () {
        this._render();
    },

    init: function () {
        var me = this,
            opt = me.options,
            el = me.element;

        //el.addClass('menu');

        me.loadData(opt.data);

        el.on('click', '.menu-title', function (event) {
            var el = $(event.currentTarget);

            var li = el.parent();

            var item = me.getItemByEvent(event);

            //alert(item);
            //me.toggleItem(item);

            li.toggleClass('open');

            if (opt.itemclick) opt.itemclick.call(me, item);

        });

    },

    _render: function () {
        var data = this.options.data || [];
        var html = this._renderItems(data, null);
        this.element.html(html);
    },

    _renderItems: function (items, parent) {
        var s = '<ul class="' + (parent ? "menu-submenu" : "menu") + '">';
        for (var i = 0, l = items.length; i < l; i++) {
            var item = items[i];
            s += this._renderItem(item);
        }
        s += '</ul>';
        return s;
    },

    _renderItem: function (item) {

        var me = this,
            hasChildren = item.children && item.children.length > 0;

        var s = '<li class="' + (hasChildren ? 'has-children' : '') + (item.expanded ? ' open' : '') + '">';        //class="menu-item" open, expanded?

        s += '<a class="menu-title" data-id="' + item.id + '" ';
        //if (item.href) {
        //   s += 'href="' + item.href + '" target="' + (item.hrefTarget || '') + '"';
        //}
        s += '>';

        s += '<i class="menu-icon fa ' + item.iconCls + '"></i>';
        s += '<span class="menu-text">' + item.text + '</span>';

        if (hasChildren) {
            s += '<span class="menu-arrow fa"></span>';
        }

        s += '</a>';

        if (hasChildren) {
            s += me._renderItems(item.children, item);
        }

        s += '</li>';
        return s;
    },

    getItemByEvent: function (event) {
        var el = $(event.target).closest('.menu-title');
        var id = el.attr("data-id");
        return this.getItemById(id);
    },

    getItemById: function (id) {
        var me = this,
            idHash = me._idHash;

        if (!idHash) {
            idHash = me._idHash = {};
            each(me.options.data);
        }

        function each(items) {
            for (var i = 0, l = items.length; i < l; i++) {
                var item = items[i];
                if (item.children) each(item.children);
                idHash[item.id] = item;
            }
        }

        return me._idHash[id];
    }

};

//MenuTip
var MenuTip = function (menu) {
    var template = '<div class="tooltip right menutip in"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>';
    var tip = $(template).appendTo(document.body);
    tip.hide();

    menu.element.on("mouseenter", ".menu-title", function (event) {
        if (!$("body").hasClass("compact")) return;

        var jq = $(event.currentTarget);
        var offset = jq.offset(),
            width = jq.outerWidth(),
            height = jq.outerHeight(),
            text = jq.text();

        tip.find(".tooltip-inner").html(text);
        tip.show();

        var tipWidth = tip.outerWidth(),
            tipHeight = tip.outerHeight();

        tip.css({ top: offset.top + height / 2 - tipHeight / 2, left: offset.left + width });

    });

    menu.element.on("mouseleave", ".menu-title", function (event) {
        tip.hide();
    });
};

//MainTabs
var MainTabs = {

    tabsId: 'mainTabs',

    init: function () {
        var me = this;
        $('#tbMainTabs #home').click(function () { me.home(); });
        $('#tbMainTabs #refresh').click(function () { me.refresh(); });
        $('#tbMainTabs #remove').click(function () { me.remove(); });
        $('#tbMainTabs #fullScreen').click(function () { me.fullScreen(); });

        var tab = this.active({ id: 'index' });
        $(tab.bodyEl).loadHtml('Pages/Dashboard.html', function () {
            //Dashboard.show();
        });
    },

    active: function (item) {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getTab(item.id);
        if (!tab) {
            tab = tabs.addTab({
                name: item.id, title: item.text, url: item.url,
                iconCls: item.iconCls, showCloseButton: true
            });
        }
        tabs.activeTab(tab);
        tab.bodyEl = tabs.getTabBodyEl(tab);
        return tab;
    },

    home: function () {
        this.active({ id: 'index' });
    },

    refresh: function () {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getActiveTab();
        tabs.reloadTab(tab);
    },

    remove: function () {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getActiveTab();
        if (tab.name !== 'index') {
            tabs.removeTab(tab);
        }
    },

    fullScreen: function () {
    }

};

//Navbar
var Navbar = {

    init: function () {
        var me = this;
        $('#tbNavbar #cache').click(function () { me.cache(); });
        $('#tbNavbar #info').click(function () { me.info(); });
        $('#tbNavbar #updPwd').click(function () { me.updPwd(); });
        $('#tbNavbar #logout').click(function () { me.logout(); });
    },

    todo: function () {
        MainTabs.active({
            id: 'todo', iconCls: 'fa-paper-plane',
            text: '代办事项', url: 'Pages/TodoView.html'
        });
    },

    cache: function () {
        Ajax.getJson('/api/User/GetCodes', function (data) {
            Code.setData(data);
            Message.tips({ content: '刷新成功！' });
        });
    },

    info: function () {
        Ajax.getJson('/api/User/GetUserInfo', function (data) {
        });
    },

    updPwd: function () {
    },

    logout: function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/signout', function () {
                location = location;
            });
        });
    }

};

$(function () {
    //menu
    var menu = new Menu('#mainMenu', {
        itemclick: function (item) {
            if (!item.children) {
                MainTabs.active(item);
            }
        }
    });

    $('.sidebar').mCustomScrollbar({ autoHideScrollbar: true });

    new MenuTip(menu);

    Ajax.getJson('/api/User/GetModules', function (result) {
        menu.loadData(result.Data.menus);
        Code.setData(result.Data.codes);
    });

    //toggle
    $('#toggle, .sidebar-toggle').click(function () {
        var body = $('body'), toggle = $('.sidebar-toggle i');
        body.toggleClass('compact');
        if (body.hasClass('compact')) {
            toggle.removeClass('fa-dedent').addClass('fa-indent');
        } else {
            toggle.removeClass('fa-indent').addClass('fa-dedent');
        }
        mini.layout();
    });

    //dropdown
    $('.dropdown-toggle').click(function (event) {
        $(this).parent().addClass('open');
        return false;
    });
    $(document).click(function (event) {
        $('.dropdown').removeClass('open');
    });

    mini.parse();

    MainTabs.init();
    Navbar.init();
});