﻿$(function () {

    $('input[onenter]').keydown(function (event) {
        if (event.which === 13) {
            event.preventDefault();
            var method = $(this).attr("onenter");
            eval(method);
        }
    });

    $('#userName').focus();

    $('#btnLogin').click(function () {
        var userName = $('#userName').val();
        var password = $('#password').val();
        if (!userName) {
            _showMessage('请输入用户名！');
            return;
        }
        if (!password) {
            _showMessage('请输入密码！');
            return;
        }

        _showMessage('');
        var $this = $(this).attr('disabled', 'disabled').val('登录中...');
        Ajax.postJson('/signin', {
            userName: userName,
            password: $.md5(password)
        }, function (result) {
            _showMessage(result.Message);
            if (result.IsValid) {
                location = result.Data;
            } else {
                $this.removeAttr('disabled').val('登录');
            }
        });

        function _showMessage(message) {
            $('#error').html(message);
        }
    });

});