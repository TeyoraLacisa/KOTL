# 📦 Руководство по настройке инвентаря

## Быстрая настройка (5 минут)

### Шаг 1: Создайте Canvas
1. В иерархии: ПКМ → UI → Canvas
2. Переименуйте в "InventoryCanvas"
3. В Canvas Scaler установите:
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: **1920 x 1080**

### Шаг 2: Создайте панель инвентаря
1. ПКМ на InventoryCanvas → UI → Panel
2. Переименуйте в "InventoryPanel"
3. В Inspector установите:
   - Anchor: **Center**
   - Width: **800**
   - Height: **600**
   - Color: темно-серый с прозрачностью (R:25, G:25, B:25, A:240)

### Шаг 3: Добавьте заголовок
1. ПКМ на InventoryPanel → UI → Text - TextMeshPro
2. Переименуйте в "Title"
3. Настройки:
   - Текст: **"ИНВЕНТАРЬ"**
   - Font Size: **36**
   - Alignment: **Center**
   - Position: вверху панели

### Шаг 4: Создайте Grid для слотов
1. ПКМ на InventoryPanel → Create Empty
2. Переименуйте в "InventoryGrid"
3. Добавьте компонент **Grid Layout Group**:
   - Cell Size: **120 x 120**
   - Spacing: **10 x 10**
   - Constraint: **Fixed Column Count = 5**
   - Child Alignment: **Upper Left**
4. Настройте RectTransform:
   - Anchors: Stretch (left:0, right:1, top:1, bottom:0)
   - Left: **20**, Right: **-20**
   - Top: **-80**, Bottom: **20**

### Шаг 5: Создайте префаб слота
1. ПКМ на InventoryGrid → UI → Image
2. Переименуйте в "ItemSlot"
3. Настройки ItemSlot:
   - Width: **120**, Height: **120**
   - Color: темно-серый (R:50, G:50, B:50, A:255)
   - Добавьте компонент **InventorySlot**

4. Внутри ItemSlot создайте:

   **a) ItemIcon (иконка предмета):**
   - UI → Image
   - RectTransform: верхняя часть слота
   - Preserve Aspect: ✓
   
   **b) ItemName (название):**
   - UI → Text - TextMeshPro
   - Position: нижняя часть слота
   - Font Size: **14**
   - Alignment: Center
   
   **c) EmptySlot (индикатор пустого слота):**
   - UI → Panel
   - Размер: весь слот
   - Color: полупрозрачный серый

5. Перетащите ItemSlot в папку **Assets/Prefabs/**
6. **Удалите** ItemSlot из сцены

### Шаг 6: Подключите InventorySystem
1. Выберите InventoryPanel
2. Добавьте компонент **InventorySystem**
3. Перетащите ссылки:
   - **Inventory Panel** → InventoryPanel
   - **Inventory Grid** → InventoryGrid  
   - **Inventory Slot Prefab** → префаб ItemSlot
   - **Max Inventory Slots** → 12

4. Скройте панель: InventoryPanel → снимите галочку в верхнем левом углу Inspector

### Шаг 7: Добавьте тестер (опционально)
1. Выберите объект **Player**
2. Добавьте компонент **InventoryTester**
3. Можете перетащить иконки предметов в поля (или оставить пустыми для теста)

---

## 🎮 Управление

| Клавиша | Действие |
|---------|----------|
| **I** | Открыть/закрыть инвентарь |
| **E** | Подобрать предмет (рядом с ним) |
| **1** | Добавить дневник (тест) |
| **2** | Добавить зажигалку (тест) |
| **3** | Добавить ключ (тест) |

---

## 🔧 Создание подбираемых предметов

1. Создайте 3D объект (Cube/Sphere/модель)
2. Добавьте компонент **PickupItem**
3. Настройте:
   - **Item Name**: "Дневник" / "Зажигалка" / "Ключ"
   - **Item Icon**: спрайт иконки
   - **Description**: описание предмета
   - **Is Key Item**: ✓
   - **Pickup Range**: 2
4. Добавьте **Box Collider** или **Sphere Collider**
5. **Обязательно**: включите **Is Trigger** ✓
6. **Важно**: У игрока должен быть тег "Player"

---

## ❌ Частые проблемы

### UI не отображается
- Проверьте, что InventoryPanel деактивирован (снята галочка)
- Canvas должен быть в режиме Screen Space - Overlay
- EventSystem должен быть в сцене

### Не подбираются предметы
- Убедитесь, что у Collider включен **Is Trigger**
- Проверьте тег игрока (**Player**)
- InventorySystem должен быть в сцене и активен

### Инвентарь не открывается
- Проверьте консоль на ошибки
- Убедитесь, что ссылки в InventorySystem подключены
- Префаб слота должен существовать в Prefabs

---

## 📝 Дополнительные скрипты

- **InventorySystem.cs** - основная система
- **InventorySlot.cs** - отдельный слот UI
- **PickupItem.cs** - подбираемый предмет
- **KeyItems.cs** - константы для ключевых предметов
- **InventoryTester.cs** - быстрое тестирование

Готово! Теперь у вас есть полностью рабочая система инвентаря! 🎉

