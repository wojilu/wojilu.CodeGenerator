using System;
using System.Collections.Generic;
using System.Text;
using wojilu.ORM;

namespace wojilu.Sample.Domain {

    [Label("文章")]
    public class Article : ObjectBase<Article> {

        [NotNull( "请填写标题" )]
        [Column( Label = "标题" )]
        public String Title { get; set; }

        [Column( Label = "分类" )]
        public Category Category { get; set; }

        [LongText, HtmlText, NotNull( "请填写内容" )]
        [Column( Label = "内容" )]
        public String Content { get; set; }

        [Column( Label = "创建时间" )]
        public DateTime Created { get; set; }
    }

}
