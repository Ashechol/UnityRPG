### 操作

v + 拖动：顶点吸附

ctrl+shift+拖动：自动吸附平面

ctrl+shift+f：设置相机到Scene位置

### 工具

polybrush：low poly风格形状编辑

probuilder：制作low poly风格物体

navigation：导航系统

cinemachine：摄像机

* FreeLook Camera：可以实现类似星际争霸摄像机的效果

animator override controller：可以直接在其他animator的基础上进行覆盖动画，从而快速构建类似角色的animation behavior：用于在动画播放自定义各种功能

#### URP

post processing：URP 的后期处理，在 Hierarchy --> Volume 中创建

shader graph：可视化的面板中完成shader功能的实现。如遮挡剔除

* Fresnel Effect

### 脚本

Camera.ScreenPointToRay：返回从摄像机通过屏幕点的光线。
Physics.Raycast：返回射线碰撞的坐标

ScriptableObject：当C#脚本继承了ScriptObject可以定义一个数据模板，在assets中快速创建模板化ScriptableObject

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("State Info")]
    public int MaxHealth;
    public int CurrentHealth;
    public int BaseDefence;
    public int CurrentDefence;
}
```

Vector3.Distance() 和 Vector3.SqrMagnitude()，后者的开销较少

在挂载脚本前就可以在inspector窗口给脚本修改信息

#### 数据保存

* PlayerPrefs + JsonUtility
  
  * JsonUtility 讲 ScriptableObject 的数据转换为 Json格式（string类型）
  
  * PlayerPrefs 使用键值形式保存数据到windows注册表

序列化反序列化

### Shader

* UV：贴图坐标相当于空间坐标XY的概念，shader中的UV输入就是贴图输入的意思

* 通道数从上往下按需要的数量取



### UI

Grid Layout Croup：管理子UI物体的排列