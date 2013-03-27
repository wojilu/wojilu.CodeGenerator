using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Attr;
using wojilu.Sample.Domain;

namespace wojilu.Web.Controller.Admin {

    public class UserController : ControllerBase {

        public void List() {
            
            set( "addLink", to( Add ) );

            DataPage<User> list = User.findPage("");
            list.Results.ForEach( x => {
                x.data.edit = to( Edit, x.Id );
                x.data.delete = to( Delete, x.Id );
            } );

            bindList( "list", "x", list.Results );

            set( "page", list.PageBar );
        }

        public void Add() {
            target( Create );

            
            bind( "x", new  User() );
        }

        [HttpPost]
        public void Create() {
            
            User data = ctx.PostObject<User>( "x" );
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.insert();
            redirect( List );
        }

        public void Edit( int id ) {
             
            target( Update, id );

            User data = User.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }
            bind( "x", data );

            
        }

        [HttpPost]
        public void Update( int id ) {
            
            User data = User.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data = ctx.PostObject( data, "x" ) as User;
            if (ctx.HasErrors) {
                echoError();
                return;
            }

            data.update();
            redirect( List );
        }

        [HttpDelete]
        public void Delete( int id ) {
             
            User data = User.findById( id );
            if (data == null) {
                echo( "数据不存在" );
                return;
            }

            data.delete();
            redirect( List );
        }

    }
}