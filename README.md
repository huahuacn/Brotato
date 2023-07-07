## 2022.3.2.f1c1

### 安装DOTS环境
一、第一种方案 在文件 `Packages\manifest.json`  中增加一行
```
"com.unity.entities.graphics": "1.0.10",
```
然后修复后续的 errcode

二、第二种方案，通过 Add package by name 来添加下列包 这是预览版本
- com.unity.entities
- com.unity.dots.editor
2. 独立构建 DOTS 项目需要为每个目标平台安装相应的平台包：

 - com.unity.platforms.android
 - com.unity.platforms.ios
 - com.unity.platforms.linux
 - com.unity.platforms.macos
 - com.unity.platforms.web
 - com.unity.platforms.windows