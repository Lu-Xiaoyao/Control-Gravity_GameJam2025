# 输入系统使用说明

## 概述
新的输入系统使用事件驱动架构，解决了场景切换时输入失效的问题。

## 架构说明

### 1. InputHandle (全局输入管理器)
- 位置：`Assets/Scripts/InputHandle.cs`
- 功能：全局单例，负责创建和管理 `InputActions` 实例
- 特点：使用 `DontDestroyOnLoad`，在场景切换时不会被销毁
- 职责：监听输入事件并触发对应的 UnityEvent

### 2. SceneInputManager (场景输入管理器)
- 位置：`Assets/Scripts/SceneInputManager.cs`
- 功能：每个场景都应该有一个这个组件
- 职责：将场景中的具体组件绑定到全局输入事件

### 3. InputTest (输入测试脚本)
- 位置：`Assets/Scripts/InputTest.cs`
- 功能：用于测试和调试输入系统
- 使用方法：添加到场景中的任意GameObject上

## 使用方法

### 步骤1：设置全局输入管理器
1. 在第一个场景中创建一个空的 GameObject
2. 添加 `InputHandle` 组件
3. 这个对象会自动使用 `DontDestroyOnLoad`，在场景切换时保持存在

### 步骤2：在每个场景中设置场景输入管理器
1. 在每个场景中创建一个空的 GameObject
2. 添加 `SceneInputManager` 组件
3. 在 Inspector 中拖拽对应的组件到相应字段：
   - `AreaShow` - 显示重力区域
   - `ResetPlayer` - 重置玩家位置
   - `ResetStars` - 重置星星位置
   - `PlayerSpeedUp` - 玩家加速

### 步骤3：输入映射
当前支持的输入：
- **F键** 或 **左扳机** - 显示重力区域
- **空格键** 或 **X键** - 重置玩家位置
- **E键** 或 **右扳机** - 玩家加速
- **T键** - 重置星星位置
- **任意键** 或 **鼠标左键** - UI下一步（对话系统）

## 优势
1. **解耦**：输入系统与具体组件解耦，使用事件系统通信
2. **持久性**：全局输入管理器在场景切换时保持存在
3. **灵活性**：每个场景可以有不同的组件配置
4. **调试友好**：包含详细的调试日志

## 注意事项
1. 确保每个场景都有 `SceneInputManager` 组件
2. 如果某个功能不需要，可以将对应字段留空
3. 调试日志会显示事件绑定和触发情况
4. 场景切换时会自动清理和重新绑定事件监听器

## 故障排除

### 如果输入不工作，请按以下步骤检查：

#### 1. 检查控制台日志
查看是否有以下日志：
- ✅ `"InputHandle: 全局InputActions实例已创建"`
- ✅ `"InputHandle: 玩家输入已启用"`
- ✅ `"SceneInputManager: 输入事件已设置完成"`
- ✅ `"InputHandle: XXX事件触发"`

#### 2. 使用InputTest脚本进行测试
1. 在场景中添加 `InputTest` 组件
2. 运行游戏，查看控制台输出
3. 按下对应按键，查看是否有 `"InputTest: XXX输入被检测到"` 日志

#### 3. 常见问题及解决方案

**问题1：没有看到 "InputHandle: 全局InputActions实例已创建" 日志**
- 解决方案：确保在第一个场景中有 `InputHandle` 组件

**问题2：没有看到 "SceneInputManager: 输入事件已设置完成" 日志**
- 解决方案：确保每个场景都有 `SceneInputManager` 组件

**问题3：看到 "SceneInputManager: XXX组件未设置" 警告**
- 解决方案：在 `SceneInputManager` 的 Inspector 中设置对应的组件引用

**问题4：看到 "InputTest: 玩家输入映射未启用" 警告**
- 解决方案：检查 `InputHandle` 是否正确启用了输入映射

**问题5：输入被检测到但没有触发事件**
- 解决方案：检查 `SceneInputManager` 中的组件引用是否正确设置

#### 4. 手动测试步骤
1. 在第一个场景中确保有 `InputHandle` 组件
2. 在每个场景中确保有 `SceneInputManager` 组件
3. 在 `SceneInputManager` 中设置所有需要的组件引用
4. 添加 `InputTest` 组件进行测试
5. 运行游戏，切换场景，测试输入功能

#### 5. 调试技巧
- 使用 `InputTest` 脚本可以快速确定问题所在
- 查看控制台日志，了解输入系统的状态
- 如果某个功能不工作，检查对应的组件引用是否设置
- 场景切换时，注意观察日志输出，确保事件重新绑定成功 