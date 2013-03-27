using System;
using System.Collections.Generic;
using System.Text;
using wojilu.ORM;

namespace wojilu.Sample.Domain {

    [Table( "Users" )]
    [Label( "用户" )]
    public class User : ObjectBase<User> {

        [NotNull( "请填写名称" ), Column( Label = "名字" )]
        public String Name { get; set; }

        [Column( Label = "密码" )]
        public String Pwd { get; set; }

        [Column( Label = "性别" )]
        public int Gender { get; set; }

        [Column( Label = "简介" )]
        public String Description { get; set; }

        [Column( Label = "创建时间" )]
        public DateTime Created { get; set; }

    }
}
