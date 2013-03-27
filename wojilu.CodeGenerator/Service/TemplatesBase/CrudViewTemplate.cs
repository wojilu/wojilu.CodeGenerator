using System;

namespace wojilu.Coder.Service {

    public class CrudViewTemplate : ICrudViewTemplate {

        public string GetAddView() {
            return @"
<div>
    <div class=""adminMainTitle""><div class=""adminSidebarTitleInternal"">添加#{mName}</div></div>

    <div class=""adminMainPanel"">
        <div class=""formPanel"">
        <form method=""post"" action=""#{ActionLink}"" class=""ajaxPostForm"">
	        <table style=""width:100%;"">
                <!-- BEGIN list -->
		        <tr>
			        <td style=""vertical-align:top;"">#{m.Label}</td>
			        <td>#{m.InputBox}</td>
		        </tr>
                <!-- END list -->
		        <tr>
			        <td>&nbsp;</td><td><input type=""submit"" class=""btn"" value=""添加数据"" /> <input type=""button"" class=""btnReturn"" value=""返回"" /></td>
		        </tr>
	        </table>
        </form>
        </div>
    </div>
</div>
";
        }

        public string GetListView() {
            return @"
<div>
    <div class=""adminMainTitle""><div class=""adminSidebarTitleInternal"">数据列表</div></div>

    <div class=""adminMainPanel"">

<table cellspacing=""1"" cellpadding=""3""  class=""dataAdminList"" id=""dataAdminList"">
    <tr class=""adminBar""><td colspan=""#{columnCount}""><div><a href=""#{addLink}""><img src=""~img/add.gif""/> 添加#{m.Name}</a></div></td></tr>
	<tr class=""tableHeader"">
        <th>编号</th>
        <!-- BEGIN header -->
		<th>#{p.Name}</th><!-- END header -->
		<th>管理</th>
	</tr>
    #{loopBegin}
	<tr class=""tableItems"">
        <td>#{x.Id}&nbsp;</td>
        <!-- BEGIN row -->
        <td>#{p.Name}&nbsp;</td><!-- END row -->
		<td>
            <a href=""#{x.data.edit}"" class=""edit"">修改</a>
            <a href=""#{x.data.delete}"" class=""deleteCmd delete"">删除</a>
        </td>
	</tr>
    #{loopEnd}
</table>
<div>
	#{page}
</div>
</div>
</div>
";
        }





    }
}

