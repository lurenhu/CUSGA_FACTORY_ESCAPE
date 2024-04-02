# FACTORY_ESCAPE

:smirk:
坚持撸铁，撸出八块腹肌:muscle:
坚持coding，码出大厂offer:alien:

### TODO
- [ ] 完善README文档
- [x] 节点编辑器制作
- [ ] 节点功能完善
- [x] 对话系统
- [x] 音乐系统
- [x] 游戏UI
- [ ] 过场演出
- [ ] 游玩流程 - GameManger
#### 节点功能制作
- [x] 普通节点功能
- [x] 合成节点功能组件（待测试）
- [x] 锁节点功能
- [x] AI锁节点功能
- [x] 角度锁节点功能
- [x] 图节点功能
- [x] 可控制节点功能
- [x] 探测节点功能
- [x] 计时器节点功能
- [x] 合成图片节点功能
- [x] QTE节点功能（待测试）
- [x] 对话节点功能
- [x] 快速点击节点功能
- [ ] 追逐节点功能
- [ ] 文本节点功能
- [x] 节点点击效果
- [x] 节点内容UI
- [ ] 节点点击文本显示
#### 对话系统功能制作
- [x] 对话UI界面
- [x] 对话系统基本逻辑
- [x] 对话导入AI文本方法
- [x] 对话导入对话文本方法
#### 音乐系统功能制作
- [x] 播放音乐方法
- [x] 播放音效方法


## 节点编辑器使用方法说明

### 创建节点编辑器
1. 找到路径$/Asset/ScriptableObjectAssets/NodeGraph/$
2. 右键文件夹，选择$"Create"->"ScriptableObjects"->"NodeGraph"$,创建节点图对象
3. 双击该节点图序列化对象，即可打开节点编辑器

### 创建编辑节点
1. 右键弹出选择菜单栏，即可创建对应节点
2. 单击已创建节点则节点变为选中状态，并可在$Inspector$面板中编辑节点属性
> 这里建议直接在Inspector面板中编辑节点属性

### 创建节点关系
1. 右键长按节点并拖动可拖处一条线，在另一节点处松开鼠标右键则可连接节点，箭头表示节点之间的父子关系

### 删除节点
1. 选中要删除的节点，在空白位置右键选择$"Delete Selected Node"$即可删除节点

### 删除节点关系
1. 选中该关系线关联的两个节点，在空白位置右键选择$"Delete Selected Node Links"$即可删除节点关系

### 相关节点创建数据编辑方法
1. 密码锁节点，创建后需要填写解锁密码，可自定义数量及数值
2. 角度锁节点，创建后需要填写解锁角度值，以X正半轴为0°，顺时针为负值[0,180]，逆时针为正值[-180,0]，可自定义数量及数值
3. 可控制节点，创建后需要填写被撞击节点id及撞击弹出节点的速度。
4. 图节点，创建后需要填入图片资源。
5. QTE节点，创建后需要选择滑动方向。
6. 快速点击节点，创建后需要填写点击次数
7. 合成节点，创建后需要填写与该节点合成的节点id
8. 探测节点，创建后需要填写探测目标节点id
9. 计时器节点，创建后需要填写开始计时的节点id（即该节点出现就会弹出计时器节点），结束计时的节点id（即该节点出现计时器就会消失），计时时间（单位：秒）
10. AI锁节点，创建后需要填写初始焦虑值与AI对话次数，以及在AI节点创建后需要建立三个节点为其子节点，从第一个索引为焦虑值[80，100]弹出，第二个索引为焦虑值[30,80]弹出，第三个索引为焦虑值[0,30]弹出
11. 对话节点，创建后需要填写对话文本，以及是否在对话结束后将当前节点替换为此节点的子节点