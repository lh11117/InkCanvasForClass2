<div align="center">

<img src="icc.png" width="128">

# InkCanvasForClass2

Elegant by Default. Based on [`InkCanvas/InkCanvasForClass`](https://github.com/InkCanvas/InkCanvasForClass) .

**这是一次基于[`InkCanvasForClass`](https://github.com/InkCanvas/InkCanvasForClass)控件的倔强**

[![UPSTREAM](https://img.shields.io/badge/UpStream-InkCanvas%2FInk--Canvas--Artistry-purple.svg "LICENSE")](https://github.com/InkCanvas/Ink-Canvas-Artistry)
![Gitea Last Commit](https://img.shields.io/gitea/last-commit/kriastans/InkCanvasForClass?gitea_url=https%3A%2F%2Fgitea.bliemhax.com%2F)
[![LICENSE](https://img.shields.io/badge/License-GPL--3.0-red.svg "LICENSE")](https://gitea.bliemhax.com/kriastans/InkCanvasForClass/src/branch/master/LICENSE)

</div>

## 链接们
- 爱发电：[https://afdian.com/a/dubi906w/](https://afdian.com/a/dubi906w/)<br/>
- 新网站：[https://icc.bliemhax.com/](https://icc.bliemhax.com/)
- QQ群聊：[https://qm.qq.com/q/ptrGkAcqMo/](https://qm.qq.com/q/ptrGkAcqMo/)
- Telegram频道：[https://t.me/InkCanvasForClass/](https://t.me/InkCanvasForClass/)

> [!important]
> 本产品与peppy的osu!以及其周边项目和产品无任何关联，若有侵权，请联系Dev协商解决。

---

- 佛系更新，仅供个人使用，禁止用于商业用途<br/>
- 如有需求请自行修改项目，欢迎您的 Pull Request 和 Issue 提出 <br/>
- **基于 <https://github.com/InkCanvas/InkCanvasForClass> 开发**

> [!important]
> **此项目仍在开发中，只会在发布正式发行版时提供Release**

## 介绍

![annotation is just a click away](./icc-github-illustrations.png)

<div align="center"><strong>InkCanvasForClass2 是基于 <a href="https://github.com/InkCanvas/InkCanvasForClass">InkCanvasForClass</a> 项目二次开发而来。该项目着重于优化原项目的 UI 设计使其变得更加便于在课堂上使用，同时也在积极开发新功能，修复原项目的不合理之处。</strong></div>

<br>

InkCanvasForClass2 通过为教学环境下班班通设备的使用做出优化，让教学效率大幅度提升：

1. 使用**屏幕批注**功能快速在任何地方书写标记
2. 使用**快捷白板**迅速开始上课，提升课堂效率
3. 使用**形状识别**自动纠正手绘形状，提升各教学场景下的作图效率
4. 使用**形状绘制**进行精确绘图，方便易上手的操作提升了教学作图效率
5. 使用**随机点名**功能可以活跃课堂气氛，提升学生主动性......

## 功能

### 基础功能
- [X] 基本的屏幕批注
- [X] 白板模式（白板模式相关功能正在大改）
- [X] PPT 模式下的每页批注

### ICC 的优化和改进
- [ ] 白板模式 UI 改进（还有背景色和稿纸模式）
- [ ] **鼠标手势**，可以让画布移动缩放，甚至是旋转，都变得更加轻松（正在开发）
- [ ] **全新的设置 UI**，正在逐步清理原项目的屎山
- [ ] **强制置顶**，保证 ICC2 永远显示在所有软件的最顶层
- [ ] **分辨率和DPI变化监听**，保证 ICC2 界面正常显示
- [ ] **高性能透明**，绘制使用多线程 UI + WindowChrome，摆脱低性能体验
- [ ] **画面冻结**，使当前画面定格
- [ ] **重写形状绘制**，让绘图体验更舒适方便，还会支持函数绘制和物理图绘制
- [ ] **PPT COM+VSTO双接口融合**，缓解 Office 和 WPS 共存导致的 COM 接口被占用的问题
- [ ] **PPT 系统优化**，高效 PPT 放映状态检测，不丢页不跳页
- [ ] **禁用边缘手势**，禁用烦人的 Windows10/11 的边缘触摸手势
- [ ] **点名器优化**，将会支持历史记录和多名单抽选，同时支持特殊配置
- [ ] **内置小工具**，内置计算器，英汉词典，倒计时，秒表，放大镜，截图等实用小工具
- [ ] **QuickPanel**，方便快捷的从 ICC2 打开所有应用和调整系统设置
- [ ] **情境化配置**，每位老师都有专属的配置文件
- [ ] **插件和脚本系统**，支持使用 dotNet Framework 开发原生扩展或使用 Javascript 开发脚本来实现自动化操作或其他扩充功能
- [ ] **形状识别**，基于微软清朝老库的手绘形状自动识别，并提供形状绘制纠正功能
- [ ] **板书库**，高效管理所有板书，课程自动分类，提供云端同步
- [ ] **自动收纳**，检测到教学软件自动开启时会自动隐藏 ICC 界面到屏幕侧边
- [ ] **自动查杀**，检测到指定软件可以让 ICC 大打出手直接查杀并自动使用 ICC 进行替代
- [ ] **桌面悬浮窗屏蔽**，隐藏画板悬浮窗，还您一个干净的电脑桌面

还有其他没有列出...

## FAQ

### 壹 - ICC对PPT的相容性如何呢？

ICC2 可以支持 WPS，但目前无法同时支持 MSOffice 和 WPS。若要启用 WPS 支持，请确保 WPS 是否在 “配置工具” 中开启了 “WPS Office 相容第三方系统和软件” 选项，该项目勾选并应用后，将无法检测到 MS Office 的COM接口。

如果您安装了“赣教通”、“畅言智慧课堂”等应用，可能会安装“畅言备课精灵”，因此会导致遗失64位的 Office COM 组件的注册且目前似乎无法修复（可以切换到新用户正常使用）。但 WPS Office 可以正常使用。

若要将 ICC2 配合 WPS 使用，可打开“WPS 演示”后，前往“文件” - “选项” ，取消勾选“单萤幕幻灯片放映时，显示放映工具栏”该项，获得更好的体验。若要将 ICC 配合 MS Office 使用，可以打开 Powerpoint，前往“选项” ，“高级”，取消勾选“显示快捷工具栏”，获得更好的体验。

### 贰 - **安装后**程序无法正常启动？

请检查你的电脑上是否安装了 `.Net Framework 4.7.2` 或更高版本。若没有，请前往官网下载。

如果程序在启动后黑屏闪退，请打开 “事件查看器” 搜索有关 InkCanvasForClass2 的错误信息并上报给开发者（可以在 GitHub 上提交 Issue，或者和开发者单独沟通）

> 遇到各种奇葩逗比问题请重启应用程式，如果不行请反馈给Dev解决！

## 特别鸣谢

🍰 **感谢下列同学对本项目的支持，因为有了你们，ICC 才能继续发展下去：**

<img src="./InkCanvasForClass/Resources/contributors.png" width=328>

## 开发

- 本项目目前开发状态

- 要在本地编译应用，您需要安装以下负载和工具：
- - [.NET Framework 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)
- - [Visual Studio](https://visualstudio.microsoft.com/)

- 对于 Visual Studio，您需要在安装时勾选以下工作负载：
- - .NET 桌面开发

## 开源许可

本项目基于 GNU General Public License Version 3 进行开源