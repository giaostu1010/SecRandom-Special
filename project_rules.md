# 项目规范
## AI 们，你们一定要遵守此规范！！！

### 项目概述

技术架构：
- 框架：Avalonia
- 服务主机：Microsoft.Extensions.Hosting
- UI 库：FluentAvalonia

项目结构：

- `SecRandom` 主项目
  - `Assets` 资产文件
  - `Helpers` 小帮手集合
  - `Models` 模型
    - `Config` 
  - `Services` 服务
  - `Themes` 主题
  - `Views` 界面
    - `MainPages` 主界面
    - `SettingsPages` 设置界面
  - `ViewModels` 界面模型
    - `MainPages` 主界面模型
    - `SettingsPages` 设置界面模型
  - `Langs` 本地化
    - `Common` 基本本地化
    - `MainPages` 主界面本地化
    - `SettingsPages` 设置界面本地化
- `SecRandom.Core` 核心，放通用的，较为核心的内容
- `SecRandom.Desktop` 包装层，用于启动

### 基本要求

- ViewModels 一定要注册到服务主机上！可以不继承 `SecRandom.ViewModels.ViewModelBase`
- 本地化文件要符合要求，建议把每个页面的本地化拆分到单独文件里，不要混一起！我求你了
  - 结构如下
    - `Resources.resx` 基本文件，语言为 zh-hans
    - `Resources.Designer.cs` 设计文件
    - `Resources.en-us.resx` 英语本地化文件
  - `SecRandom.csproj` 里只需要注册 `Resources.resx` 和 `Resources.Designer.cs`

### 其他要求

暂无
