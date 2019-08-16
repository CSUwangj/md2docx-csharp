# md2docx-csharp

使用C#开发的将markdown转换为docx的工具。

该项目是[CSUIS-md2docx](https://github.com/CSUwangj/CSUIS-md2docx)的一个子项目，目前1.0版已经完工，下一步动向见TODO。

## 快速开始

下载release中的`Release.zip`，解压后在解压的目录中，命令行使用，若无参数，则默认输入文件为同文件夹下的`test.md`。

运行参数为`md2docx.exe <md input path> <docx output path>`，若不指定第一个参数，则默认读入同目录的`test.md`，若不指定第二个参数，则按照`<name><ID><filename>.docx`存储文件，各参数来自于markdown文件的yaml头部。

## 编译

使用Visual Studio(>=2017)打开.sln后进行编译，如果有缺少的库、VS支持，VS（理论上）会进行提示并可以进行相对自动化的安装。

## Markdown语法

见[specification](./docs/spec.md)

## TODO

- [ ] 页眉
- [ ] 页脚
- [ ] 将格式等设置改为配置文件而非硬编码
- [ ] 将parser更换成一个标准更接近GFM的parser
- [ ] 图片
- [ ] 表格
