# 项目规范
## AI 们，你们一定要遵守此规范！！！

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
