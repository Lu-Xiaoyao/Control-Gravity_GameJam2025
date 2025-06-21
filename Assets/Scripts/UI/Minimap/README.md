# 小地图系统 - 完整解决方案

## 📋 概述
为您的2D重力控制游戏提供了一套完整的小地图解决方案，包含简单版和高级版两个版本，满足不同需求。

## 🎯 功能特性

### 基础功能
- ✅ 左上角显示小地图
- ✅ 自动显示玩家位置（蓝色图标）
- ✅ 显示星球位置（绿色图标）
- ✅ 显示门位置（黄色图标）
- ✅ 显示星星位置（白色图标）
- ✅ 按M键切换显示/隐藏
- ✅ 自动检测地图边界
- ✅ 玩家图标显示朝向

### 高级功能（AdvancedMinimap）
- ✅ 鼠标拖拽移动小地图
- ✅ 鼠标滚轮缩放
- ✅ 点击小地图获取世界坐标
- ✅ 可调节缩放范围
- ✅ 更多自定义选项

## 📁 文件结构

```
UI/
├── SimpleMinimap.cs          # 简单小地图系统
├── AdvancedMinimap.cs        # 高级小地图系统
├── MinimapSetup.cs           # 快速设置工具
├── Minimap.cs                # 相机渲染版小地图（备选）
├── 小地图使用说明.md          # 详细使用说明
└── README.md                 # 本文件
```

## 🚀 快速开始

### 方法一：自动设置（推荐）
1. 在场景中创建空GameObject
2. 添加 `MinimapSetup` 脚本
3. 勾选 `Auto Setup`
4. 运行游戏即可

### 方法二：手动设置
1. 确保游戏对象有正确标签：
   - 玩家：`Player`
   - 星球：`Planet`
   - 门：`Door`
   - 星星：`Star`
2. 创建空GameObject，添加 `SimpleMinimap` 或 `AdvancedMinimap` 脚本
3. 在Inspector中调整参数

## 🎮 使用方法

### 基本操作
- **M键**：切换小地图显示/隐藏
- **鼠标拖拽**（高级版）：移动小地图位置
- **鼠标滚轮**（高级版）：缩放小地图

### 代码调用
```csharp
// 获取小地图管理器
SimpleMinimap minimap = FindObjectOfType<SimpleMinimap>();

// 切换显示
minimap.ToggleMinimap();

// 设置显示状态
minimap.SetMinimapVisible(true);

// 动态添加星球
minimap.AddPlanet(planetTransform);
```

## ⚙️ 配置选项

### SimpleMinimap 参数
- **Minimap Size**: 小地图大小
- **Minimap Position**: 小地图位置
- **Background Color**: 背景颜色
- **Border Color**: 边框颜色
- **Icon Size**: 图标大小
- **Map Radius**: 地图半径
- **Toggle Key**: 切换键

### AdvancedMinimap 额外参数
- **Can Drag**: 是否允许拖拽
- **Can Zoom**: 是否允许缩放
- **Can Click**: 是否允许点击
- **Min/Max Zoom**: 缩放范围
- **Current Zoom**: 当前缩放

## 🎨 自定义

### 图标预制体
1. 创建UI Image对象
2. 设置图标图片和颜色
3. 制作成预制体
4. 在Inspector中拖拽到对应字段

### 预设配置
使用 `MinimapSetup` 脚本可以快速应用预设配置：
- 默认小地图
- 大号小地图
- 紧凑小地图

## 🔧 故障排除

### 常见问题
1. **小地图不显示**
   - 检查Canvas是否存在
   - 确认Is Visible是否开启
   - 检查对象标签设置

2. **图标位置不正确**
   - 调整Map Radius参数
   - 检查对象Transform组件
   - 确认标签设置正确

3. **性能问题**
   - 减少图标数量
   - 调整更新频率
   - 使用对象池

## 📝 版本说明

### v1.0 - 基础版本
- SimpleMinimap：基础小地图功能
- 自动标签检测
- 基本图标显示

### v1.1 - 高级版本
- AdvancedMinimap：交互式小地图
- 拖拽和缩放功能
- 快速设置工具

## 🤝 贡献指南
如果您想为这个项目做出贡献：
1. 报告Bug或提出建议
2. 提交代码改进
3. 添加新功能
4. 改进文档

## 📄 许可证
本项目采用MIT许可证，您可以自由使用、修改和分发。

## 📞 支持
如果您在使用过程中遇到问题：
1. 查看使用说明文档
2. 检查故障排除部分
3. 查看Unity控制台错误信息
4. 确认Unity版本兼容性

---

**祝您游戏开发愉快！** 🎮✨ 