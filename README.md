# md2docx-csharp![](https://img.shields.io/github/license/CSUwangj/md2docx-csharp)[![Build status](https://ci.appveyor.com/api/projects/status/github/CSUwangj/md2docx-csharp?branch=master&svg=true)](https://ci.appveyor.com/project/CSUwangj/md2docx-csharp/branch/master)![](https://img.shields.io/github/v/release/CSUwangj/md2docx-csharp?include_prereleases)

使用C#开发的将markdown转换为docx的工具。

该项目是[CSUIS-md2docx](https://github.com/CSUwangj/CSUIS-md2docx)的一个子项目，目前2.1版已经完工，下一步动向见TODO。

下载: [latest release](https://github.com/CSUwangj/md2docx-csharp/releases) | [latest CI build (master)](https://ci.appveyor.com/api/projects/CSUwangj/md2docx-csharp/artifacts/md2docx/bin/md2docx.zip)(文档未完成)

## 快速开始

下载release中的`Release.zip`，解压后在解压的目录中，命令行使用。也可以将程序路径加入PATH方便使用。

无参数使用时除了提示信息外相当于`md2docx.exe -i input.md -c config.json -o <id><name><filename>.docx`。

请注意图片路径的问题。

## 编译

### Visual Studio

使用Visual Studio(>=2017)打开.sln后进行编译，如果有缺少的库、VS支持，VS（理论上）会进行提示并可以进行相对自动化的安装。

### MSBuild

`MSBuild.exe md2docx.sln /p:Configuration=Debug /p:Platform="Any CPU"`

`MSBuild.exe md2docx.sln /p:Configuration=Release /p:Platform="Any CPU"`

输出路径为`md2docx\bin\(Debug|Release)`

## Markdown语法

见[specification](./docs/spec.md)

## 配置文件

schema文件为[schema](./docs/schema.json)，示例文件为[default config](./examples/config.json)。

## 文档

在我的[博客](https://csuwangj.github.io/%E7%BC%96%E7%A8%8B%E6%98%AF%E5%BE%88%E5%A5%BD%E7%8E%A9%E7%9A%84-md2docx%E6%98%AF%E6%80%8E%E4%B9%88%E5%86%99%E5%87%BA%E6%9D%A5%E7%9A%84/)中对写这个程序的过程做了一个简介，同时总结了一下用到的资料，暂且先用它吧。

## TODO

未实现的TODO按照实现可能性降序排列

- [x] 加入测试（并重构）
- [x] 将格式等设置改为配置文件而非硬编码
- [x] 部署CI
- [x] 图片
- [x] 页眉
- [x] 页脚
- [ ] 将parser更换成一个标准更接近GFM的parser
- [ ] 根据文件设定路径
- [ ] 表格
- [ ] 列表
- [ ] 公式
- [ ] 图形化
