layui.define('common', function (exports) {
    var $ = layui.jquery,
        layer = layui.layer,
        form = layui.form,
        table = layui.table,
        common = layui.common;

    function getCurTab() {
        return top.layui.admin.getCurTab();
    }

    function Grid(option) {
        var name = option.name,
            config = option.config,
            toolbar = option.toolbar;
        var tableIns = null, _where = {}, keyId = name + '_key';
        var _this = this;

        $.extend(config, {
            elem: '#' + name, skin: 'line',
            even: true, page: true, cellMinWidth: 80
        });

        if (toolbar) {
            if (!config.toolbar) {
                var tab = getCurTab();
                var tbHtml = '<div class="layui-btn-container">';
                if (tab.module && tab.module.children) {
                    tab.module.children.forEach(function (d) {
                        tbHtml += ('<button class="layui-btn layui-btn-sm" lay-event="' + d.code + '">');
                        tbHtml += ('<i class="layui-icon ' + d.icon + '"></i>' + d.title);
                        tbHtml += '</button>';
                    });
                }
                if (config.url || option.showSearch) {
                    //tbHtml += '<span class="grid-search-adv">高级</span>';
                    tbHtml += '<span class="grid-search">';
                    tbHtml += '  <input type="text" id="' + keyId + '" placeholder="请输入查询关键字" class="layui-input" autocomplete="off">';
                    tbHtml += '  <i class="layui-icon layui-icon-search" lay-event="search"></i>';
                    tbHtml += '</span>';

                    toolbar.search = function () {
                        var key = $('#' + keyId).val();
                        _this.reload({ key: key });
                        $('#' + keyId).val(key);
                    }
                }
                tbHtml += '</div>';
                config.toolbar = tbHtml;
            }

            table.on('toolbar(' + name + ')', function (obj) {
                var type = obj.event;
                var rows = table.checkStatus(name).data;
                toolbar[type] && toolbar[type].call(this, new GridManager(_this, rows));
            });
            table.on('tool(' + name + ')', function (obj) {
                var type = obj.event;
                toolbar[type] && toolbar[type].call(this, new GridManager(_this, [obj.data]));
            });
        }

        if (config.url) {
            config.method = 'post';
            config.autoSort = false;
            tableIns = table.render(config);

            table.on('sort(' + name + ')', function (obj) {
                var key = $('#' + keyId).val();
                _where.field = obj.field;
                _where.order = obj.type;
                tableIns.reload({ initSort: obj, where: _where });
                $('#' + keyId).val(key);
            });
        }

        this.setData = function (data) {
            config.data = data;
            tableIns = table.render(config);
        }

        this.where = {};
        this.reload = function (where) {
            $.extend(this.where, where);
            _where.query = JSON.stringify(this.where);
            tableIns.reload({ where: _where });
        }
    }

    function GridManager(grid, rows) {
        this.grid = grid;
        this.rows = rows;
        this.row = rows[0];

        this.selectRow = function (callback) {
            if (!rows || rows.length === 0 || rows.length > 1) {
                layer.msg('请选择一条记录！');
                return;
            }

            var row = rows[0];
            callback && callback({ grid: grid, rows: rows, row: row, ids: [row.Id] });
        }

        this.selectRows = function (callback) {
            if (!rows || rows.length === 0) {
                layer.msg('请至少选择一条记录！');
                return;
            }

            var ids = rows.map(d => d.Id);
            callback && callback({ grid: grid, rows: rows, ids: ids });
        }

        this.editRow = function (form, ext) {
            this.selectRow(function (e) {
                form.show(e.row, ext);
            });
        }

        function deleteData(e, url, callback) {
            var length = e.rows.length;
            var msg = length > 1 ? ('所选的' + length + '条记录') : '该记录';
            common.confirm('确定要删除' + msg + '吗？', function () {
                common.post(url, { data: JSON.stringify(e.ids) }, function () {
                    e.grid.reload();
                    callback && callback(e);
                });
            });
        }

        this.deleteRow = function (url, callback) {
            this.selectRow(function (e) {
                deleteData(e, url, callback);
            });
        }

        this.deleteRows = function (url, callback) {
            this.selectRows(function (e) {
                deleteData(e, url, callback);
            });
        }
    }

    function Form(option) {
        var name = option.name,
            config = option.config,
            toolbar = option.toolbar;

        var _this = this;
        if (config.area) {
            var btn = [], handler = {};
            if (toolbar) {
                for (var i = 0; i < toolbar.length; i++) {
                    btn.push(toolbar[i].text);
                    var evt = i === 0 ? 'yes' : ('btn' + (i + 1));
                    var hdl = toolbar[i].handler;
                    handler[evt] = function (index, layero) {
                        hdl && hdl.call(this, new FormManager(_this));
                    };
                }
            }

            handler['btn' + (btn.length + 1)] = function (index) {
                layer.close(index);
            };
            btn.push('关闭');

            $.extend(config, { btn: btn }, handler);
        }

        var index = 0;
        this.show = function (data, ext) {
            data = data || option.defData;
            if (config.area) {
                var title = option.title;
                if (!title) {
                    var tab = getCurTab();
                    title = tab ? tab.title : '';
                }
                ext = ext || {};
                var info = ext.title || '';
                var actionName = data.Id === '' ? '【新增】' : '【编辑】';
                if (info.indexOf('【') > -1) {
                    actionName = '';
                }
                config.title = title + actionName + info;
                config.success = function () {
                    form.render(null, name);
                    _this.setData(data, config.init);
                }
                index = common.open(config);
            } else {
                this.setData(data, config.init);
            }
        }

        this.close = function () {
            if (index > 0) {
                layer.close(index);
            }
        }

        this.getField = function (id) {
            return $('[lay-filter="' + name + '"]').find('[name="' + id + '"]');
        }

        this.getData = function () {
            return form.val(name);
        }

        this.setData = function (data, callback) {
            form.val(name, data);
            var e = { form: _this, data: data };
            config.setData && config.setData(e);
            callback && callback(e);
        }
    }

    function FormManager(form) {
        this.form = form;

        this.save = function (url, callback) {
            var data = form.getData();
            common.post(url, { data: JSON.stringify(data) }, function (id) {
                data.Id = id;
                form.setData(data);
                form.close();
                callback && callback();
            });
        }
    }

    function selectIcon(icon, callback) {
        var data = [
            'heart-fill', 'heart', 'light', 'time', 'bluetooth', 'at', 'mute',
            'mike', 'key', 'gift', 'email', 'rss', 'wifi', 'logout',
            'android', 'ios', 'windows', 'transfer', 'service', 'subtraction', 'addition',
            'slider', 'print', 'export', 'cols', 'screen-restore', 'screen-full', 'rate-half',
            'rate', 'rate-solid', 'cellphone', 'vercode', 'login-wechat', 'login-qq', 'login-weibo',
            'password', 'username', 'refresh-3', 'auz', 'spread-left', 'shrink-right', 'snowflake',
            'tips', 'note', 'home', 'senior', 'refresh', 'refresh-1', 'flag',
            'theme', 'notice', 'website', 'console', 'face-surprised', 'set', 'template-1',
            'app', 'template', 'praise', 'tread', 'male', 'female', 'camera',
            'camera-fill', 'more', 'more-vertical', 'rmb', 'dollar', 'diamond', 'fire',
            'return', 'location', 'read', 'survey', 'face-smile', 'face-cry', 'cart-simple',
            'cart', 'next', 'prev', 'upload-drag', 'upload', 'download-circle', 'component',
            'file-b', 'user', 'find-fill', 'loading', 'loading-1', 'add-1', 'play',
            'pause', 'headset', 'video', 'voice', 'speaker', 'fonts-del', 'fonts-code',
            'fonts-html', 'fonts-strong', 'unlink', 'picture', 'link', 'face-smile-b', 'align-left',
            'align-right', 'align-center', 'fonts-u', 'fonts-i', 'tabs', 'radio', 'circle',
            'edit', 'share', 'delete', 'form', 'cellphone-fine', 'dialogue', 'fonts-clear',
            'layer', 'date', 'water', 'code-circle', 'carousel', 'prev-circle', 'layouts',
            'util', 'templeate-1', 'upload-circle', 'tree', 'table', 'chart', 'chart-screen',
            'engine', 'triangle-d', 'triangle-r', 'file', 'set-sm', 'reduce-circle', 'add-circle',
            '404', 'about', 'up', 'down', 'left', 'right', 'circle-dot',
            'search', 'set-fill', 'group', 'friends', 'reply-fill', 'menu-fill', 'log',
            'picture-fine', 'face-smile-fine', 'list', 'release', 'ok', 'help', 'chat',
            'top', 'star', 'star-fill', 'close-fill', 'close', 'ok-circle', 'add-circle-fine'
        ];
        var content = '<ul class="icon-list">';
        data.forEach(function (d) {
            var icon1 = 'layui-icon-' + d;
            content += '<li id="' + icon1 + '"';
            content += icon1 === icon ? ' class="active">' : '>';
            content += '<i class="layui-icon ' + icon1 + '"></i>';
            content += '</li>';
        });
        content += '</ul>';
        common.open({
            title: '选择图标',
            area: ['400px', '250px'],
            content: content,
            btn: ['确定', '关闭'],
            yes: function (index) {
                var icon = $('.icon-list li.active').attr('id');
                layer.close(index);
                callback && callback(icon);
            },
            btn2: function (index) {
                layer.close(index);
            },
            success: function (layero, index) {
                $('.icon-list li').click(function () {
                    $('.icon-list li').removeClass('active');
                    $(this).addClass('active');
                });
            }
        });
    }

    exports('frame', {

        common: common,

        selectIcon: function (icon, callback) {
            selectIcon(icon, callback);
        },

        grid: function (option) {
            return new Grid(option);
        },

        form: function (option) {
            return new Form(option);
        },

        open: function (option) {
            return common.open(option);
        },

        confirm: function (message, callback) {
            common.confirm(message, callback);
        },

        post: function (url, data, callback) {
            common.post(url, data, callback);
        }

    });
});