# md2docx-csharp

使用C#开发的将markdown转换为docx的工具。

该项目是[CSUIS-md2docx](https://github.com/CSUwangj/CSUIS-md2docx)的一个子项目，目前1.0版已经完工，下一步动向见TODO。

## 快速开始

下载release中的`Release.zip`，解压后在解压的目录中，命令行使用，若无参数，则默认输入文件为同文件夹下的`test.md`。

运行参数为`md2docx.exe <md input path> <docx output path>`，若不指定第一个参数，则默认读入同目录的`test.md`，若不指定第二个参数，则按照`<name><ID><filename>.docx`存储文件，各参数来自于markdown文件的yaml头部。

## 编译

### Visual Studio

使用Visual Studio(>=2017)打开.sln后进行编译，如果有缺少的库、VS支持，VS（理论上）会进行提示并可以进行相对自动化的安装。

### MSBuild

`MSBuild.exe Solution.sln /p:Configuration=Debug /p:Platform="Any CPU"`

`MSBuild.exe Solution.sln /p:Configuration=Release /p:Platform="Any CPU"`

输出路径为`md2docx\bin\(Debug|Release)`

## Markdown语法

见[specification](./docs/spec.md)

## 文档

TBD

在我的[博客](https://csuwangj.github.io/%E7%BC%96%E7%A8%8B%E6%98%AF%E5%BE%88%E5%A5%BD%E7%8E%A9%E7%9A%84-md2docx%E6%98%AF%E6%80%8E%E4%B9%88%E5%86%99%E5%87%BA%E6%9D%A5%E7%9A%84/)中对写这个程序的过程做了一个简介，同时总结了一下用到的资料，暂且先用它吧。

## TODO

- [x] 加入测试（并重构）
- [ ] 部署CI
- [ ] 将parser更换成一个标准更接近GFM的parser
- [x] 将格式等设置改为配置文件而非硬编码
- [ ] 页眉
- [ ] 页脚
- [ ] 图片
- [ ] 表格
