using System;
using System.Collections;
using System.IO;
using System.Text;

using wojilu.ORM;
using wojilu.Web;
using wojilu.Web.Mvc;

namespace wojilu.Coder.Service {

    public class CodeService : ICodeService {

        private static readonly ILog logger = LogManager.GetLogger( typeof( CodeService ) );

        private string targetPath;
        private string namespaceName;

        private string viewPath;
        private string controllerPath;

        public IViewTemplate viewTemplate { get; set; }
        public IControllerTemplate controllerTemplate { get; set; }

        public CodeService( ) {
            this.viewTemplate = new ViewNormal();
            this.controllerTemplate = new ControllerTemplate();
        }

        public ICodeService Init( string codePath, string nsName ) {
            this.targetPath = codePath;
            this.namespaceName = nsName;
            return this;
        }

        public void Make() {

            this.prepareDir();

            this.makeLayoutController();
            this.makeAllController();

            this.makeLayoutView();
            this.makeAllView();
        }

        public void MakeSingle( string typeName ) {

            EntityInfo ei = Entity.GetInfo( typeName );
            if (ei == null) {
                logger.Error( "MakeSingle error: EntityInfo  is null" );
                return;
            }

            this.prepareDir();

            this.makeOneController( ei );

            this.makeActionViews( ei );
        }


        private void prepareDir() {

            if (!Directory.Exists( this.targetPath )) {
                Directory.CreateDirectory( this.targetPath );
            }

            this.viewPath = Path.Combine( this.targetPath, "view" );
            this.viewPath = Path.Combine( this.viewPath, "Admin" );
            if (!Directory.Exists( this.viewPath )) {
                Directory.CreateDirectory( this.viewPath );
            }

            this.controllerPath = Path.Combine( this.targetPath, "controller" );
            this.controllerPath = Path.Combine( this.controllerPath, "Admin" );
            if (!Directory.Exists( this.controllerPath )) {
                Directory.CreateDirectory( this.controllerPath );
            }
        }

        //---------------------controller---------------------------------------------------

        private void makeAllController() {
            foreach (DictionaryEntry entry in MappingClass.Instance.ClassList) {
                EntityInfo ei = entry.Value as EntityInfo;
                this.makeOneController( ei );
            }
        }

        private void makeOneController( EntityInfo ei ) {

            String[] arrTypeItem = ei.FullName.Split( '.' );

            String domainNamespace = strUtil.TrimEnd( ei.FullName, "." + arrTypeItem[arrTypeItem.Length - 1] );

            string codeList = this.getListCode( ei );
            string codeAdd = this.getAddCode( ei );
            string codeCreate = this.getCreateCode( ei );
            string codeEdit = this.getEditCode( ei );
            string codeUpdate = this.getUpdateCode( ei );
            string codeDel = this.getDeleteCode( ei );

            Template template = new Template();
            template.InitContent( controllerTemplate.GetController() );

            template.Set( "domainNamespace", domainNamespace );
            template.Set( "namespace", this.namespaceName );
            template.Set( "controllerName", ei.Name );

            template.Set( "domainCamelName", strUtil.GetCamelCase( ei.Name ) );


            template.Set( "listCode", codeList );
            template.Set( "addCode", codeAdd );
            template.Set( "createCode", codeCreate );
            template.Set( "editCode", codeEdit );
            template.Set( "updateCode", codeUpdate );
            template.Set( "deleteCode", codeDel );

            wojilu.IO.File.Write( Path.Combine( this.controllerPath, string.Format( "{0}Controller.cs", ei.Name ) ), template.ToString() );
        }


        private string getAddCode( EntityInfo ei ) {
            Template template = new Template();
            template.InitContent( controllerTemplate.GetAddAction() );
            IBlock block = template.GetBlock( "editor" );
            String eiName = strUtil.GetCamelCase( ei.Name );

            String entityProperty = "";
            foreach (EntityPropertyInfo ep in ei.SavedPropertyList) {
                if (ep.IsLongText) {
                    block.Set( "Name", "x." + ep.Name );
                    block.Next();
                }

                else if (ep.IsEntity) {
                    String epName = getEntityNameSimple( ep );
                    if (epName != null) {
                        entityProperty += string.Format( tab3 + "dropList( \"x.{0}\", {1}.findAll(), \"{2}=Id\", null );" + Environment.NewLine, ep.Name, ep.EntityInfo.Name, epName );
                    }
                }
            }
            template.Set( "entityProperty", entityProperty );

            return template.ToString();
        }


        private string getCreateCode( EntityInfo ei ) {
            Template t = new Template();
            t.InitContent( controllerTemplate.GetCreateAction() );
            this.populateTemplate( ei, t );
            return t.ToString();
        }

        private string getDeleteCode( EntityInfo ei ) {
            Template template = new Template();
            template.InitContent( controllerTemplate.GetDeleteAction() );
            template.Set( "m.Name", ei.Name );
            return template.ToString();
        }

        private string getEditCode( EntityInfo ei ) {
            Template template = new Template();
            template.InitContent( controllerTemplate.GetEditAction() );
            template.Set( "m.Name", ei.Name );

            String eiName = strUtil.GetCamelCase( ei.Name );
            template.Set( "domainCamelName", eiName );

            String entityProperty = "";
            IBlock block = template.GetBlock( "editor" );
            foreach (EntityPropertyInfo ep in ei.SavedPropertyList) {
                if (ep.IsLongText) {
                    block.Set( "Name", "x." + ep.Name );
                    block.Set( "PName", ep.Name );
                    block.Next();
                }
                else if (ep.IsEntity) {
                    String epName = getEntityNameSimple( ep );
                    if (epName != null) {
                        entityProperty += string.Format( tab3 + "dropList( \"x.{0}\", {1}.findAll(), \"{2}=Id\", data.{3}.Id );" + Environment.NewLine, ep.Name, ep.EntityInfo.Name, epName, ep.Name );
                    }
                }
            }
            template.Set( "entityProperty", entityProperty );
            return template.ToString();
        }

        private string getListCode( EntityInfo ei ) {
            Template template = new Template();
            template.InitContent( controllerTemplate.GetListAction() );
            template.Set( "model.Name", ei.Name );
            template.Set( "model.LName", strUtil.GetCamelCase( ei.Name ) );
            return template.ToString();
        }

        private string getUpdateCode( EntityInfo ei ) {
            Template t = new Template();
            t.InitContent( controllerTemplate.GetUpdateAction() );
            this.populateTemplate( ei, t );
            return t.ToString();
        }

        private void populateTemplate( EntityInfo ei, Template t ) {
            StringBuilder reqBuilder = new StringBuilder();
            StringBuilder validBuilder = new StringBuilder();
            StringBuilder setBuilder = new StringBuilder();
            foreach (EntityPropertyInfo info in ei.SavedPropertyList) {
                if (info.Name.Equals( "Id" )) {
                    continue;
                }
                if (info.Type == typeof( string )) {

                    reqBuilder.Append( this.tab3 );
                    reqBuilder.AppendFormat( "string {0} = ctx.Post(\"{1}\");", strUtil.GetCamelCase( info.Name ), info.Name );
                    reqBuilder.Append( Environment.NewLine );

                    validBuilder.Append( this.tab3 );
                    validBuilder.AppendFormat( "if ( strUtil.IsNullOrEmpty( {0} ) )", strUtil.GetCamelCase( info.Name ) );
                    validBuilder.Append( Environment.NewLine );
                    validBuilder.Append( this.tab4 );
                    validBuilder.AppendFormat( "errors.Add( \"请填写{0}\" );", info.Label );
                    validBuilder.Append( Environment.NewLine );

                    setBuilder.Append( this.tab3 );
                    setBuilder.AppendFormat( "data.{0} = {1};", info.Name, strUtil.GetCamelCase( info.Name ) );
                    setBuilder.Append( Environment.NewLine );
                }
                else if (info.Type == typeof( int )) {

                    reqBuilder.Append( this.tab3 );
                    reqBuilder.AppendFormat( "int {0} = ctx.PostInt(\"{1}\");", strUtil.GetCamelCase( info.Name ), info.Name );
                    reqBuilder.Append( Environment.NewLine );

                    setBuilder.Append( this.tab3 );
                    setBuilder.AppendFormat( "data.{0} = {1};", info.Name, strUtil.GetCamelCase( info.Name ) );
                    setBuilder.Append( Environment.NewLine );
                }
                else if (info.Type == typeof( DateTime )) {

                    reqBuilder.Append( this.tab3 );
                    reqBuilder.AppendFormat( "DateTime {0} = ctx.PostTime(\"{1}\");", strUtil.GetCamelCase( info.Name ), info.Name );
                    reqBuilder.Append( Environment.NewLine );

                    setBuilder.Append( this.tab3 );
                    setBuilder.AppendFormat( "data.{0} = {1};", info.Name, strUtil.GetCamelCase( info.Name ) );
                    setBuilder.Append( Environment.NewLine );
                }
            }
            t.Set( "setPostValue", reqBuilder.ToString() );
            t.Set( "validPostValue", validBuilder.ToString() );
            t.Set( "setValue", setBuilder.ToString() );
            t.Set( "m.Name", ei.Name );
        }



        //-----------------------view----------------------------------------------

        private void makeAllView() {
            foreach (DictionaryEntry entry in MappingClass.Instance.ClassList) {
                EntityInfo ei = entry.Value as EntityInfo;
                this.makeActionViews( ei );
            }
        }

        private void makeActionViews( EntityInfo ei ) {
            string path = Path.Combine( this.viewPath, ei.Name );
            if (!Directory.Exists( path )) {
                Directory.CreateDirectory( path );
            }
            this.makeView_Action_List( path, ei );
            this.makeView_Action_Add( path, ei );
            this.makeView_Action_Edit( path, ei );
        }

        private void makeView_Action_Add( string modelControllerDir, EntityInfo ei ) {
            string fileContent = this.getEditPage( ei, false );
            wojilu.IO.File.Write( Path.Combine( modelControllerDir, "Add.html" ), fileContent );
        }

        private void makeView_Action_Edit( string modelControllerDir, EntityInfo ei ) {
            string fileContent = this.getEditPage( ei, true ).Replace( "添加", "修改" );
            wojilu.IO.File.Write( Path.Combine( modelControllerDir, "Edit.html" ), fileContent );
        }


        private void makeView_Action_List( string modelControllerDir, EntityInfo ei ) {
            Template template = new Template();
            template.InitContent( viewTemplate.GetListView() );
            template.Set( "m.Name", ei.Label );
            IBlock block = template.GetBlock( "header" );
            IBlock block2 = template.GetBlock( "row" );
            int columnCount = this.setPropertyList( block, ei, true );
            this.setPropertyList( block2, ei, false );
            template.Set( "loopBegin", "<!-- BEGIN list -->" );
            template.Set( "loopEnd", "<!-- END list -->" );
            template.Set( "columnCount", columnCount );

            wojilu.IO.File.Write( Path.Combine( modelControllerDir, "List.html" ), template.ToString() );
        }


        private string getEditPage( EntityInfo ei, bool isEdit ) {

            Template template = new Template();
            template.InitContent( viewTemplate.GetAddView() );
            template.Set( "mName", ei.Label );

            IBlock block = template.GetBlock( "list" );
            foreach (EntityPropertyInfo info in ei.SavedPropertyList) {
                string rule = "";
                string msg = "";
                string valid = "";
                string tip = "";

                if (!info.Name.Equals( "Id" )) {
                    block.Set( "m.Label", info.Label );
                    block.Set( "m.InputBox", this.getInputBox( info, isEdit, ref valid, ref rule, ref msg, ref tip ).ToString().Replace( "name=", tip + " name=" ) + this.setValid( valid, msg, rule ) );
                    block.Next();
                }
            }
            return template.ToString();
        }

        private string setValid( string valid, string msg, string rule ) {
            return string.Format( "\n<span {0} {1} {2}></span>", valid, rule, msg ); ;
        }

        private object getInputBox( EntityPropertyInfo ep, bool isEdit, ref string valid, ref string rule, ref string msg, ref string tip ) {
            string controlName = "x." + ep.Name;
            string valueStr = string.Empty;
            if (isEdit) valueStr = "#{" + controlName + "}";
            if (rft.GetAttribute( ep.Property, typeof( NotNullAttribute ) ) != null) {
                valid = "class=\"valid\"";
                msg = string.Format( "msg=\"{0}\"", rft.GetPropertyValue( rft.GetAttribute( ep.Property, typeof( NotNullAttribute ) ), "Message" ) );
            }
            if (rft.GetAttribute( ep.Property, typeof( EmailAttribute ) ) != null) {
                rule = "rule=\"email\"";
                msg = string.Format( "msg=\"输入Email格式有误\"", ep.Label == null ? ep.Name : ep.Label );
            }
            if (rft.GetAttribute( ep.Property, typeof( TinyIntAttribute ) ) != null) {
                rule = "rule=\"int\"";
            }
            if (rft.GetAttribute( ep.Property, typeof( MoneyAttribute ) ) != null) {
                rule = "rule=\"money\"";
            }
            tip = string.Format( " class=\"tipInput\" tip=\"请输入{0}\"", ep.Label == null ? ep.Name : ep.Label );
            if (ep.IsLongText) {
                if (rft.GetAttribute( ep.Property, typeof( HtmlTextAttribute ) ) != null) {
                    return "#{Editor}";
                }
                else {
                    return Html.TextArea( controlName, valueStr, "width:95%;height:50px;" );
                }
            }

            if (ep.IsEntity) {
                return "#{" + controlName + "}";
            }

            if (ep.Type == typeof( DateTime ) && !isEdit) {
                return Html.TextInput( controlName, DateTime.Now.ToString( "g" ), "width:150px;" );
            }

            if (ep.Type == typeof( int ) || ep.Type == typeof( decimal )) {
                return Html.TextInput( controlName, valueStr, "width:60px;" );
            }

            if (ep.Type == typeof( string ) && ep.SaveAttribute != null && ep.SaveAttribute.Length > 50) {
                return Html.TextInput( controlName, valueStr, "width:450px;" );
            }

            return Html.TextInput( controlName, valueStr, "width:250px;" );
        }

        private int setPropertyList( IBlock block, EntityInfo ei, bool isLabel ) {
            int columnCount = 0;
            foreach (EntityPropertyInfo info in ei.SavedPropertyList) {
                columnCount = columnCount + 1;
                if (info.Name.Equals( "Id" )) {
                    continue;
                }
                if (isLabel) {
                    block.Set( "p.Name", info.Label );
                }
                else {
                    block.Set( "p.Name", "#{x." + info.Name + "}" );
                }
                block.Next();
            }
            return columnCount + 1;// add admin column
        }



        //---------------------------layout------------------------------------------------


        private void makeLayoutController() {
            Template template = new Template();
            template.InitContent( controllerTemplate.GetLayoutController() );

            IBlock block = template.GetBlock( "list" );
            foreach (DictionaryEntry entry in MappingClass.Instance.ClassList) {

                EntityInfo ei = entry.Value as EntityInfo;
                String lmName = strUtil.GetCamelCase( ei.Name );

                block.Set( "mName", ei.Name );
                block.Set( "lmName", lmName );
                block.Next();
            }

            wojilu.IO.File.Write( Path.Combine( this.controllerPath, "LayoutController.cs" ), template.ToString() );
        }

        private void makeLayoutView() {
            Template template = new Template();
            template.InitContent( viewTemplate.GetLayoutView() );

            IBlock block = template.GetBlock( "list" );
            foreach (DictionaryEntry entry in MappingClass.Instance.ClassList) {

                EntityInfo ei = entry.Value as EntityInfo;
                String lmName = strUtil.GetCamelCase( ei.Name );

                block.Set( "m.Name", ei.Label );
                block.Set( "m.LName", lmName );
                block.Set( "m.AdminLink", "#{" + lmName + ".AdminLink}" );
                block.Next();
            }

            String path = Path.Combine( this.viewPath, "Layout" );
            if (!Directory.Exists( path )) Directory.CreateDirectory( path );

            wojilu.IO.File.Write( Path.Combine( path, "Layout.html" ), template.ToString() );
        }

        //-------------------------------------------------------------------------------------------


        private string getEntityName( EntityPropertyInfo ep ) {

            String pn = getEntityNameSimple( ep );
            return pn == null ? ep.Name : ep.Name + "." + pn;
        }

        private string getEntityNameSimple( EntityPropertyInfo ep ) {

            if (ep.EntityInfo.GetProperty( "Name" ) != null) return "Name";
            if (ep.EntityInfo.GetProperty( "Title" ) != null) return "Title";

            return null;
        }


        private string tab1 {
            get { return "    "; }
        }

        private string tab2 {
            get { return "        "; }
        }

        private string tab3 {
            get { return "            "; }
        }

        private string tab4 {
            get { return "                "; }
        }




    }
}

