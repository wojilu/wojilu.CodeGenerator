using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Sample.Domain;

namespace wojilu.Web.Controller.Admin {

    public class CategoryController : ControllerBase {

        public void List() {
            
            set( "addLink", to( Add ) );

            DataPage<Category> list = Category.findPage("");
            bindList( list.Results );
            set( "page", list.PageBar );
        }

        private void bindList( List<Category> list ) {
            IBlock block = getBlock( "list" );
            foreach (Category data in list) {
                
                block.Set( "d.Name", data.Name );
                block.Set( "d.Description", strUtil.CutString( data.Description, 30 ) );
                block.Set( "d.Created", data.Created );
                block.Set( "d.Id", data.Id );
                block.Set( "d.LinkEdit", to( Edit, data.Id ) );
                block.Set( "d.LinkDelete", to( Delete, data.Id ) );
                block.Next();
            }
        }

        public void Add() {
            target( Create );

            editorFull( "category.Description", "", "350px" );
            
            bind( "category", new  Category() );
        }

        [HttpPost]
        public void Create() {
            
            Category data = ctx.PostValue<Category>();
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.insert();
            redirect( List );
        }

        public void Edit( int id ) {
             
            target( Update, id );

            Category data = Category.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }
            bind( "category", data );

            editorFull( "category.Description", data.Description, "350px" );
            
        }

        [HttpPost]
        public void Update( int id ) {
            
            Category data = Category.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data = ctx.PostValue( data ) as Category;
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.update();
            redirect( List );
        }

        [HttpDelete]
        public void Delete( int id ) {
             
            Category data = Category.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data.delete();
            redirect( List );
        }

    }
}