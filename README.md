这是 wojilu framework 代码自动生成器，欢迎使用。  
有任何问题或建议，欢迎到 <http://www.wojilu.com> 论坛反馈，谢谢。

**主要功能**

1. 根据领域模型，生成控制器(controller)的 CRUD 代码(增删改查)，包括模板文件； 
2. 同时提供对所有数据“增加、修改、删除”等的在线操作。

**基本用法**
 
1. 将 wojilu.CodeGenerator.dll 拷贝到你的web项目的bin目录下 
2. 在 web.config 中的 InjectAssembly 中增加 wojilu.CodeGenerator 
3. 配置 /framework/config/orm.config，确保已经填写 AssemblyList 项
4. 访问你的网站，在根目录后面输入 /Code/Index.aspx
5. 点击左侧命令，生成代码。代码在 /_autocode/ 中
6. 更多图文教程，请访问 <http://www.wojilu.com/forum1/topic/2406>

【常见错误】  

* 症状：无法加载程序集:xxx, 请检查名称是否正确
* 原因：尚未配置orm.config，请先配置orm.config