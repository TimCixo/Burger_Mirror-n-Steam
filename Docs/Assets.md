# Assets

## Сцени

- `Assets/InternalAssets/Scenes/SCN_Main.unity` - основна робоча сцена прототипу

## Префаби персонажів

- `ACT_Host` - варіант гравця-хоста
- `ACT_Client` - варіант гравця-клієнта
- `ACT_NPC` - варіант неігрового персонажа
- `TPL_Actor` - базовий шаблон персонажа

## Префаби середовища

- `ENV_Floor` - модуль підлоги
- `ENV_Wall` - модуль стіни

## Holdable та stackable об'єкти

- `HBLE_Plate` - тарілка для зборки
- `HBLE_BottomBun` - нижня булка
- `HBLE_Patty` - котлета
- `HBLE_Cheese` - сир
- `HBLE_TopBun` - верхня булка
- `TPL_Holdable` - базовий holdable prefab
- `TPL_Ingredient` - базовий ingredient prefab

## Пропси сцени

- `PROP_WorkTable` - робочий стіл
- `PROP_Stove` - плита
- `PROP_PickupStation` - точка видачі/отримання предметів

## Матеріали

### Actors

- `MAT_Host`
- `MAT_Client`
- `MAT_NPC`

### Architecture

- `MAT_Floor`
- `MAT_Wall`

### Holdable

- `MAT_Bun`
- `MAT_Patty`
- `MAT_Cheese`
- `MAT_Plate`

### Props

- `MAT_StoveTop`

## Технічні ресурси

- `Assets/InternalAssets/InputActions.inputactions` - карта керування гравця
- `Assets/Settings/*` - URP renderer assets і pipeline settings

## Примітка

Назви префабів уже відображають роль ресурсу в проєкті. Для подальшого розширення варто зберігати поділ на `Actors`, `Architecture`, `Holdable`, `Props`, `Scenes`, `Scripts`, `Materials`.
