using System;
using System.Collections.Generic;
using System.Text;

namespace wojilu.Coder.Service {

    public class MakeCodeTemplate {

        public static string GetLayoutView() {
            return @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">

<head>
<title>wojilu framework 代码生成 & 对象数据管理</title>

<script src=""~js/jquery.js?#{jsVersion}"" type=""text/javascript""></script>
<script src=""~js/wojilu.common.js?#{jsVersion}"" type=""text/javascript""></script>
<script src=""~js/lang.zh-cn.js?#{jsVersion}"" type=""text/javascript""></script>
<script src=""~js/wojilu.common.admin.js?#{jsVersion}"" type=""text/javascript""></script>
<style>

body{margin:0px;}
body, table {font-size:12px;line-height:1.3; font-family:verdana;color:#2B2B2B;}
img {border:0px;}
ul,li {margin:0px;padding:0px; list-style:none;}

a { text-decoration: none;}
a:hover { text-decoration: underline;}
.link {cursor:pointer;}
a, .link { color: #005eac; }
textarea {padding:2px;}
a:hover { color:#5195ce}

#masthead { height:50px;}
    #headText {float:left; font-size:18px; font-family:微软雅黑; font-weight:bold;margin:10px 20px 20px 5px;}
    #memberInfo {float:right; margin:12px 20px; font-weight:bold;font-size:14px;}

#container {width:100%;}
    #sidebar {	background:#efefef; width:200px;vertical-align:top;}
    #contentMain { vertical-align:top;}

#container table {border-collapse:collapse; border:1px #aaa solid;}
#container th,td {border:1px #aaa solid; padding:1px;}
#container table.formTable {width:98%;margin:10px;}

.tdOID {text-align:center;}
.tdOperation {text-align:center;}
.pagebar {margin:5px 10px;}

.codeWelcome {margin:30px;}
.codeCmd {margin:5px 10px 2px 10px;}

#footer {padding:5px 10px; text-align:right;}

</style>
</head>

<body>

<div style=""background:#fff;padding:10px;"">
<div id=""masthead"">
	<div id=""headText""><a href=""#{adminLink}"">""我记录""框架 · 代码自动生成器</a></div>
	<div id=""memberInfo""></div>
</div>

<table id=""container"">
	<tr>
		<td id=""sidebar"">
		<div style=""margin: 20px 10px;"">
            <div style=""margin:5px 0px 10px 0px;font-size:14px;font-weight:bold;""><a href=""#{makeCodeLink}"" class=""postCmd"">生成代码</a></div>
			<div style=""margin:20px 0px 10px 0px;font-size:12px;font-weight:bold;"">数据管理(#{objCount})</div>
			<ul style=""margin-left:10px;"">
				<!-- BEGIN list -->
				<li style=""margin-bottom:3px;""><a href=""#{t.Link}"">#{t.Name}</a></li>
				<!-- END list -->
			</ul>
		</div>
		</td>
		<td id=""contentMain"">#{layout_content}</td>
	</tr>
</table>

<div id=""footer"">copyright &copy; 2010 - 2012  <a href=""http://www.wojilu.com"" target=""_blank"">www.wojilu.com</a></div>

</div>
</body>
</html>
";
        }

        public static String GetProductInfo() {
            return @"
<div style=""margin:-20px 10px 30px 30px;"">有任何问题或建议，欢迎到 <a href=""http://www.wojilu.com"" target=""_blank"">www.wojilu.com</a> 反馈，谢谢。</div>
<div style=""margin:0px 10px 30px 30px;"">
    <div style=""font-weight:bold;"">主要功能：</div>
    <div style=""margin:10px;"">
        <div>1) 根据领域模型生成所有控制器(controller)的相应 CRUD 代码(增、删、改、查)，包括对应的视图模板文件；</div>
        <div>2) 对所有数据进行在线管理。</div>
    </div>
    <div style=""margin:10px;"">请点击左侧链接进行操作。</div>
</div>
";
        }

        public static String GetWelcome() {
            return "<div class=\"codeWelcome\">这是 wojilu framework 代码自动生成器，欢迎使用。</div>";
        }

    }
}
