# Architecture

## Огляд

Проєкт побудований як Unity sandbox-prototype з простим поділом на дані, поведінку персонажа, інтерактивні предмети та сценові префаби.

## Структура

- `Assets/InternalAssets/Scenes` - сцени
- `Assets/InternalAssets/Prefabs` - префаби акторів, середовища, предметів і пропсів
- `Assets/InternalAssets/Materials` - матеріали для візуального розділення сутностей
- `Assets/InternalAssets/Physics Materials` - physics materials для collider-налаштувань runtime-фізики
- `Assets/InternalAssets/Prefabs/UI` - prefab-и HUD та інших UI-елементів сцени
- `Assets/InternalAssets/Scripts/Actors` - поведінка гравця
- `Assets/InternalAssets/Scripts/Holdable` - системи утримання предметів і колізій
- `Assets/InternalAssets/Scripts/Stackable` - стекування інгредієнтів і дані бургера
- `Assets/InternalAssets/Scripts/Utilities` - дрібні допоміжні утиліти
- `Assets/TextMesh Pro` - імпортовані runtime та editor resources для UI і тексту

## Ключові модулі

### Actors

- `IaMovement`
- `IaRotation`
- `IaJump`
- `IaHoldObject`
- `IaZoomObject`
- `IaThrowObject`

Ці компоненти формують базовий controller персонажа.

### Holdable

- `HoldableObject` керує physics-follow режимом утримання, не переводячи предмет у жорсткий parent-attach
- `CollisionCache` кешує colliders для групового ігнорування колізій
- `CollisionUtility` централізує `Physics.IgnoreCollision`

### Stackable

- `StackableObject` описує елемент, який можна додати до стеку
- `StackHolder` приймає нові об'єкти в trigger-зоні
- `BurgerData` накопичує склад поточного бургера

## Поточні межі архітектури

- мережевий шар ще не введений у кодову базу
- runtime bootstrap і scene flow поки прості та зосереджені на одній сцені
- UI поки обмежений одним HUD prefab у сцені, а game state management ще не винесений в окремі модулі
