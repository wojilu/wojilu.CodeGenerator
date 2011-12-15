using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using wojilu.IO;
using wojilu.ORM;
using wojilu.Coder.Service;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.DI;

namespace wojilu.Web.Controller {

    public class CodeController : ControllerBase {

        private static readonly ILog logger = LogManager.GetLogger( typeof( CodeController ) );

        public CodeController() {

            List<String> rootNs = MvcConfig.Instance.RootNamespace;
            foreach (String ns in rootNs) {

                String topLayoutStr = strUtil.Join( ns, "LayoutController", "." );
                Type topLayoutType = null;
                ObjectContext.Instance.TypeList.TryGetValue( topLayoutStr, out topLayoutType );
                if (topLayoutType != null) {
                    HideLayout( topLayoutType );
                }
            }

        }

        public override void Layout() {

            utils.setCurrentView( new Template() );
            utils.getCurrentView().InitContent( MakeCodeTemplate.GetLayoutView() );

            set( "adminLink", to( Index ) );
            set( "makeCodeLink", to( MakeCode ) );

            IBlock block = getBlock( "list" );
            List<String> keys = getOrderedKeys();
            foreach (String key in keys) {
                EntityInfo ei = MappingClass.Instance.ClassList[ key] as EntityInfo;
                block.Set( "t.Name", ei.Label );
                block.Set( "t.FullName", key );
                block.Set( "t.TableName", ei.TableName );
                block.Set( "t.Link", to( Model ) + "?typeName=" + ei.Type.FullName );
                block.Next();
            }
            set( "objCount", keys.Count );
        }

        private List<String> getOrderedKeys() {
            List<String> keys = new List<string>();
            foreach (DictionaryEntry entry in MappingClass.Instance.ClassList) {
                keys.Add( entry.Key.ToString() );
            }
            keys.Sort();
            return keys;
        }

        public void Index() {
            actionContent( getWelcome() + MakeCodeTemplate.GetProductInfo() );
        }


        [HttpPost]
        public void MakeCode() {
            string codePath = PathHelper.Map( autoCodePath );
            string nsName = "wojilu.Web.Controller.Admin";
            new CodeService( codePath, nsName ).Make();
            actionContent( "<div style=\"margin:30px;\">代码自动生成成功，请到 <span style=\"color:red;font-weight:bold;\">" + codePath + "</span> 查看。生成时间：<span style=\"color:blue;\">" + DateTime.Now + "</span></div>" );
        }

        public void Model() {

            String typeName = ctx.Get( "typeName" );
            if (strUtil.IsNullOrEmpty( typeName )) {
                actionContent( getWelcome() );
                return;
            }

            EntityInfo ei = Entity.GetInfo( typeName );
            if (ei == null) {
                actionContent( getWelcome() );
                return;
            }

            string condition = "";
            string strPropertyValue = "";
            string pName = ctx.Get( "p" );
            int objId = ctx.GetInt( "id" );
            if (strUtil.HasText( pName ) && (objId > 0)) {
                EntityPropertyInfo property = ei.GetProperty( pName );
                if ((property != null) && property.IsEntity) {
                    condition = pName + ".Id=" + objId;
                    IEntity pValue = ndb.findById( property.Type, objId );
                    if (pValue != null) {
                        if (property.EntityInfo.GetProperty( "Name" ) != null) {
                            strPropertyValue = pValue.get( "Name" ).ToString();
                        }
                        else if (property.EntityInfo.GetProperty( "Title" ) != null) {
                            strPropertyValue = pValue.get( "Title" ).ToString();
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( "<div class='codeCmd'><a href=\"{0}\">+ 添加 {1}</a>", to( Add ) + "?typeName=" + typeName ,ei.Label );
            if (strPropertyValue.Length > 0) {
                sb.Append( "<span style=\"margin-left:15px;\">当前过滤:<span style=\"color:red;\">" );
                sb.Append( strPropertyValue );
                sb.Append( "</span></span>" );
                sb.AppendFormat( "<a href=\"{0}\" style=\"margin-left:10px;\">全部</a>", to( Model ) + "?typeName=" + typeName );
            }
            sb.Append( "</div>" );
            this.setHeader( ei, sb );

            IPageList pages = ndb.findPage( Entity.GetInfo( typeName ).Type, condition );
            foreach (IEntity obj in pages.Results) {
                this.setRow( ei, sb, obj );
            }
            sb.Append( "</table>" );
            sb.AppendFormat( "<div class=\"pagebar\">{0}</div>", pages.PageBar );
            actionContent( sb.ToString() );
        }

        private void setHeader( EntityInfo ei, StringBuilder sb ) {
            sb.Append( "<table border=\"1\" cellspacing=\"0\" cellpadding=\"0\" class='formTable'><tr>" );
            sb.Append( "<th>Id</th>" );
            foreach (EntityPropertyInfo pInfo in ei.SavedPropertyList) {
                if (!pInfo.Name.Equals( "Id" )) {
                    sb.AppendFormat( "<th>{0}</th>", pInfo.Label );
                }
            }
            sb.Append( "<td class=\"tdOperation\">管理</a></td>" );
            sb.Append( "</tr>" );
        }

        private void setRow( EntityInfo ei, StringBuilder sb, IEntity o ) {

            sb.Append( "<tr>" );
            sb.AppendFormat( "<td class=\"tdOID\">{0}</td>", o.Id );
            foreach (EntityPropertyInfo info in ei.SavedPropertyList) {

                if (info.Name.Equals( "Id" )) continue;

                object pvalue = o.get( info.Name );
                if (info.IsEntity) {
                    pvalue = this.getEntityPropertyName( info, pvalue );
                }
                else if (info.IsLongText) {
                    pvalue = strUtil.ParseHtml( pvalue, 20 ) + "...";
                }
                sb.AppendFormat( "<td>{0}&nbsp;</td>", pvalue );
            }
            sb.AppendFormat( "<td class=\"tdOperation\"><a href=\"{0}\">修改</a> <a href=\"{1}\" class=\"deleteCmd\">删除</a></td>", to( Edit, o.Id ) + "?typeName=" + ei.FullName, to( Delete, o.Id ) + "?typeName=" + ei.FullName );
            sb.Append( "</tr>" );
        }


        //---------------------------------------------------------------------------------------------------------


        public void Add() {

            String typeName = ctx.Get( "typeName" );

            EntityInfo ei = Entity.GetInfo( typeName );
            Html html = new Html();

            html.Code( "<form methond=\"post\" class=\"ajaxPostForm\" action=\"" + to( Insert ) + "\">" );
            html.Code( string.Format( "<div class='codeCmd'><a href='{0}'>返回列表</a></div>", to( Model ) + "?typeName=" + typeName ) );
            html.Code( "<table border=\"1\" cellspacing=\"0\" cellpadding=\"0\" class='formTable'>" );
            foreach (EntityPropertyInfo propertyInfo in ei.SavedPropertyList) {
                if (propertyInfo.Name.Equals( "Id" )) {
                    continue;
                }
                string txtValue = "";
                if ((propertyInfo.Type == typeof( int )) || (propertyInfo.Type == typeof( decimal ))) {
                    txtValue = "0";
                }
                else if (propertyInfo.Type == typeof( DateTime )) {
                    txtValue = DateTime.Now.ToString();
                }
                string style = "";
                if ((propertyInfo.SaveAttribute == null) || ((propertyInfo.SaveAttribute != null) && (propertyInfo.SaveAttribute.Length > 100))) {
                    style = "width:400px;";
                }
                string control = Html.TextInput( propertyInfo.Name, txtValue, style );
                if (propertyInfo.IsLongText) {

                    if (rft.GetAttribute( propertyInfo.Property, typeof( HtmlTextAttribute ) ) != null) {

                        control = Editor.NewOne( propertyInfo.Name, "", "200px", editorJsPath, MvcConfig.Instance.JsVersion, Editor.ToolbarType.Full ).ToString();
                    }
                    else {

                        control = "<div>" + Html.TextArea( propertyInfo.Name, "", "width:98%; height:80px;" ) + "</div>";

                    }
                }
                else if (propertyInfo.IsEntity) {
                    string textField = getDropText( propertyInfo );
                    if (textField.Length > 0) {
                        IList list = ndb.findAll( propertyInfo.Type );
                        if (list.Count > 0) {
                            control = Html.DropList( list, propertyInfo.Name, textField, "Id", null );
                        }
                    }
                }
                html.Code( string.Format( "<tr><td class=\"mLable\">{0}</td><td>{1} {2}</td></tr>", propertyInfo.Label, control, this.getErrorInfo( propertyInfo.Name ) ) );
            }
            html.Code( "</td></tr></table>" );
            html.Code( "<div style=\"margin:5px 10px;\">" );
            html.Submit( "添加数据" );
            html.Code( "<input type=\"button\" onclick=\"history.back();\" value=\"返回\" style=\"margin-left:15px;\" />" );
            html.HiddenInput( "typeName", ei.FullName );
            html.Code( "</div>" );
            html.FormEnd();
            actionContent( html.ToString() );
        }

        [HttpPost]
        public void Insert() {

            string typeName = ctx.Post( "typeName" );
            if (strUtil.IsNullOrEmpty( typeName )) {
                echo( "未知类型数据" );
                return;
            }

            EntityInfo ei = Entity.GetInfo( typeName );
            IEntity obj = Entity.New( typeName );

            this.setPostValues( ei, obj );

            if (ctx.HasErrors) {
                echoError();
                return;
            }

            Result result = db.insert( obj );

            if (result.HasErrors) {
                echoError( result );
                return;
            }

            redirectUrl( to( Model ) + "?typeName=" + typeName );

        }

        //-----------------------------------------------------------------------------------------------------------

        public void Edit( int id ) {

            string typeName = ctx.Get( "typeName" );
            if (strUtil.IsNullOrEmpty( typeName )) {
                echo( "未知类型数据" );
                return;
            }

            EntityInfo info = Entity.GetInfo( typeName );
            if (info == null) {
                echo( "未知类型数据" );
                return;
            }

            IEntity obj = ndb.findById( info.Type, id );
            this.showEdit( obj );
        }

        private void showEdit( IEntity obj ) {

            EntityInfo entityInfo = Entity.GetInfo( obj );
            Html html = new Html();

            html.Code( "<form methond=\"post\" class=\"ajaxPostForm\" action=\"" + to( Update, obj.Id ) + "\">" );

            html.Code( string.Format( "<div class='codeCmd'><a href='{0}'>返回列表</a></div>", to( Model ) + "?typeName=" + entityInfo.FullName ) );
            html.Code( "<table border=\"1\" cellspacing=\"0\" cellpadding=\"0\" class='formTable'>" );
            foreach (EntityPropertyInfo pInfo in entityInfo.SavedPropertyList) {

                if (pInfo.Name.Equals( "Id" )) continue;

                string str = this.getPropertyValue( obj, pInfo );
                string style = "";
                if ((pInfo.SaveAttribute == null) || ((pInfo.SaveAttribute != null) && (pInfo.SaveAttribute.Length > 100))) {
                    style = "width:400px;";
                }
                string control = Html.TextInput( pInfo.Name, str, style );
                if (pInfo.IsLongText) {
                    control = Editor.NewOne( pInfo.Name, str, "200px", editorJsPath, MvcConfig.Instance.JsVersion, Editor.ToolbarType.Full ).ToString();
                }
                else if (pInfo.IsEntity) {
                    string textField = getDropText( pInfo );
                    if (textField.Length > 0) {
                        IList list = ndb.findAll( pInfo.Type );
                        if (list.Count > 0) {
                            IEntity objP = obj.get( pInfo.Name ) as IEntity;
                            if (objP != null) {
                                control = Html.DropList( list, pInfo.Name, textField, "Id", objP.Id );
                            }
                        }
                    }
                }
                html.Code( string.Format( "<tr><td class=\"mLable\">{0}</td><td>{1} {2}</td></tr>", pInfo.Label, control, this.getErrorInfo( pInfo.Name ) ) );
            }
            html.Code( "</td></tr></table>" );
            html.Code( "<div style=\"margin:5px 10px;\">" );
            html.Submit( "修改数据" );
            html.Code( "<input type=\"button\" onclick=\"history.back();\" value=\"返回\" style=\"margin-left:15px;\" />" );
            html.HiddenInput( "typeName", entityInfo.FullName );
            html.Code( "</div>" );
            html.FormEnd();
            actionContent( html.ToString() );
        }

        [HttpPost]
        public void Update( int id ) {

            string typeName = ctx.Post( "typeName" );
            if (strUtil.IsNullOrEmpty( typeName )) {
                echo( "未知类型数据" );
                return;
            }

            EntityInfo ei = Entity.GetInfo( typeName );
            if (ei == null) {
                echo( "未知类型数据" );
                return;
            }

            IEntity obj = ndb.findById( ei.Type, id );
            this.setPostValues( ei, obj );

            if (ctx.HasErrors) {
                echoError();
                return;
            }

            Result result = db.update( obj );
            if (result.HasErrors) {
                echoError( result );
                return;
            }

            redirectUrl( to( Model ) + "?typeName=" + typeName );
        }

        //-----------------------------------------------------------------------------------------------------------

        [HttpDelete]
        public void Delete( int id ) {

            string typeName = ctx.Get( "typeName" );
            if (strUtil.IsNullOrEmpty( typeName )) {
                echo( "未知类型数据" );
                return;
            }

            EntityInfo info = Entity.GetInfo( typeName );
            if (info == null) {
                echo( "未知类型数据" );
                return;
            }

            IEntity obj = ndb.findById( info.Type, id );
            if (obj == null) {
                echo( "数据不存在" );
                return;
            }

            int affected = db.delete( obj );
            if (affected <= 0) {
                echoError( "删除失败，请查看日志" );
                return;
            }

            echoRedirect( "删除成功", to( Model ) + "?typeName=" + typeName );
        }

        //-----------------------------------------------------------------------------------------------------------

        private void setPostValues( EntityInfo ei, IEntity obj ) {

            foreach (EntityPropertyInfo pInfo in ei.SavedPropertyList) {

                if (pInfo.Name.Equals( "Id" )) continue;

                string postValue = ctx.Post( pInfo.Name );

                if (pInfo.IsLongText) {

                    if (rft.GetAttribute( pInfo.Property, typeof( HtmlTextAttribute ) ) != null) {
                        obj.set( pInfo.Name, ctx.PostHtmlAll( pInfo.Name ) );
                    }
                    else {
                        obj.set( pInfo.Name, postValue );
                    }
                }
                else if (pInfo.IsEntity) {
                    int postInt = cvt.ToInt( postValue );
                    IEntity pValue = Entity.New( pInfo.Type.FullName );
                    pValue.Id = postInt;
                    obj.set( pInfo.Name, pValue );
                }
                else {
                    object pValue2 = this.getPropertyValue( pInfo, postValue );
                    obj.set( pInfo.Name, pValue2 );
                }
            }
        }

        private Dictionary<String, String> getConfig() {
            string absolutePath = PathHelper.Map( "_admin/admin.config" );
            if (!File.Exists( absolutePath )) {
                return new Dictionary<string, string>();
            }
            return cfgHelper.Read( absolutePath );
        }

        private static string getDropText( EntityPropertyInfo ep ) {

            if (ep.EntityInfo.GetProperty( "Name" ) != null) return "Name";
            if (ep.EntityInfo.GetProperty( "Title" ) != null) return "Title";
            return "";
        }

        private string getEntityPropertyName( EntityPropertyInfo ep, object propertyValue ) {

            IEntity pValue = propertyValue as IEntity;
            if (pValue == null) return "";

            string val = null;

            EntityInfo ei = Entity.GetInfo( pValue );

            if ((ei.GetProperty( "Name" ) != null) && (pValue.get( "Name" ) != null)) {
                val = pValue.get( "Name" ).ToString();
            }
            else if ((ei.GetProperty( "Title" ) != null) && (pValue.get( "Title" ) != null)) {
                val = pValue.get( "Title" ).ToString();
            }

            if (strUtil.HasText( val )) {
                String lnk = to( Model ) + "?typeName=" + ep.ParentEntityInfo.FullName + "&p=" + ep.Name + "&id=" + pValue.Id;
                return "<a title=\"" + ep.Type.FullName + ", Id=" + pValue.Id + "\" href=\"" + lnk + "\">" + val + "</a>";
            }
            else {
                return ep.Type.Name + "_" + pValue.Id;
            }
        }

        private string getErrorInfo( string propertyName ) {
            foreach (string str in ctx.errors.Errors) {
                if (str.Equals( propertyName )) {
                    return "<span style='color:red;'>请填写此项</span>";
                }
            }
            return "";
        }

        private string getPropertyValue( IEntity obj, EntityPropertyInfo ep ) {
            object val = obj.get( ep.Name );
            if (val == null) {
                return "";
            }
            return val.ToString();
        }

        private object getPropertyValue( EntityPropertyInfo ep, string postValue ) {
            if (ep.Type == typeof( int )) {
                return cvt.ToInt( postValue );
            }
            if (ep.Type == typeof( decimal )) {
                return Convert.ToDecimal( postValue );
            }
            if (ep.Type == typeof( DateTime )) {
                return cvt.ToTime( postValue );
            }
            return postValue;
        }


        private String getWelcome() {
            return MakeCodeTemplate.GetWelcome();
        }

        private string appPath {
            get { return SystemInfo.ApplicationPath; }
        }

        private string autoCodePath {
            get { return strUtil.Join( appPath, "_autocode/" ); }
        }

        private string editorJsPath {
            get { return sys.Path.Editor; }
        }

    }
}

