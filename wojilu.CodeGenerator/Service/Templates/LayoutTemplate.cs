using System;
using System.Collections.Generic;
using System.Text;

namespace wojilu.Coder.Service {
    
    public class LayoutTemplate {

        public static string GetAdminLayoutAction() {

            return @"
using System;
using System.Collections.Generic;
using System.Text;
using wojilu.Web.Mvc;

namespace wojilu.Web.Controller.Admin {

    public class LayoutController : ControllerBase {

        public override void Layout() {

            <!-- BEGIN list -->
            set( ""#{lmName}.AdminLink"", to( new Admin.#{mName}Controller().List ) );<!-- END list -->

        }
    }
}
";

        }

        public static string GetAdminLayoutView() {
            return @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>文章管理</title>
    <link href=""~css/wojilu._common.css?v=#{cssVersion}"" rel=""stylesheet"" type=""text/css"" />
    <link href=""~css/wojilu._admin.css?v=#{cssVersion}"" rel=""stylesheet"" type=""text/css"" />
    <link href=""~css/wojilu.site.admin.css?v=#{cssVersion}"" rel=""stylesheet"" type=""text/css"" />
<script>var __funcList = []; var _run = function (aFunc) { __funcList.push(aFunc); }; var require = { urlArgs: 'v=#{jsVersion}' };</script>
    
<script  type=""text/javascript"">
    _run(function () {
        var topNavHeight = $('#headerContainer').height();
        $('.adminContainer').height($(window).height() - topNavHeight - 2);
    });
</script>
    
</head>
<body>


<div id=""headerContainer"">

	<table style=""width: 100%"" id=""header"" cellpadding=""0"" cellspacing=""0"">
		<tr>
			<td id=""logo""><a href="""">wojilu 数据后台管理</a></td>
			<td id=""loginUser""></td>
		</tr>
	</table>

	<div id=""menuContainer"">

		<div id=""menu"">
			<div style=""float:left"">
			    <a href=""~/""><img src=""~img/home.gif"" /> 首页</a>                
                <span>数据管理</span>                             
			</div>
		</div>

	</div>
</div>
<table border=""1"" cellpadding=""0"" cellspacing=""0"" class=""adminContainer"">
	<tr>
		<td class=""adminSidebar"">
			<div class=""adminSidebarTitle""><div class=""adminSidebarTitleInternal"">数据管理</div></div>

            <ul><!-- BEGIN list -->
                <li><a href=""#{m.AdminLink}""><img src=""~img/right.gif"" /> #{m.Name}管理</a></li><!-- END list -->
            </ul>            
            
		</td>
		<td class=""adminMain"" id=""main"">#{layout_content}</td>
	</tr>
</table>
<script data-main=""~js/main"" src=""~js/lib/require-jquery-wojilu.js?v=#{jsVersion}""></script>
<script>require([""wojilu._admin""])</script>
</body>
</html>
";
        }


    }
}
