using System;
using System.Collections.Generic;
using System.Text;
using wojilu.ORM;

namespace wojilu.Sample.Domain {

    [Label("文章")]
    public class Article : ObjectBase<Article> {

        [NotNull( "请填写标题" )]
        public String Title { get; set; }
        public Category Category { get; set; }

        [LongText, HtmlText, NotNull( "请填写内容" )]
        public String Content { get; set; }

        public DateTime Created { get; set; }
    }

}
