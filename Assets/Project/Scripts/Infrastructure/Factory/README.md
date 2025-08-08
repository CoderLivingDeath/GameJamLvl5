# Система ввода для GameJamLvl5

Это система обработки ввода, реализованная для Unity с использованием **новой Input System**, фреймворка **Zenject** для внедрения зависимостей и собственной шины событий (**EventBus**) для передачи событий. Она предоставляет структурированный и удобный способ подписываться и обрабатывать игровые и UI-события ввода в проекте.

---

## Обзор

Основной класс — `CodeGeneratedInputServiceFactory`, который создаёт экземпляр `InputService`, настроенный с подписками на действия ввода. Он связывает действия из `InputSystem_Actions` (сгенерированных по Input System) с обработчиками, которые в свою очередь посылают типизированные события через EventBus.

Такая архитектура позволяет:

- Разделить логику ввода и бизнес-логику через события.
- Легко подписываться на события ввода с помощью интерфейсов (например, `IGameplay_MovementEventHandler`).
- Чётко разделять обработку ввода для геймплея и UI.
- Удобно расширять и поддерживать систему ввода.

---

## Компоненты

### `CodeGeneratedInputServiceFactory`

- Реализует интерфейс `IFactory`.
- Содержит константы ключей для путей действий ввода.
- Через конструктор получает `DiContainer` для разрешения зависимостей.
- В методе `Create()` разрешает `EventBus` и `InputSystem_Actions`.
- Настраивает подписки на действия ввода в методе `CreateSubscribers`.
- Обработчики читают значения из контекста ввода и вызывают события через EventBus.

### Ключи действий ввода

Константы путей к действиям в Input System:

- Игровой процесс:
  - `"gameplay/movement"`
  - `"gameplay/interact"`
  - `"gameplay/mouse/position"`
- UI:
  - `"ui/up"`
  - `"ui/down"`
  - `"ui/left"`
  - `"ui/right"`
  - `"ui/mouse/position"`
  - `"ui/mouse/left_click"`
  - `"ui/mouse/right_click"`

### Интеграция с EventBus

- События вызываются через `eventBus.RaiseEvent(handler => handler.Метод(...))`.
- Подписчики событий реализуют соответствующие интерфейсы, например:
  - `IGameplay_MovementEventHandler`
  - `IUI_UpEventHandler` и др.

### Обработчики ввода

Внутренние функции-делегаты вызываются при срабатывании Input Actions:

```csharp
void Gameplay_Movement_Handler(InputAction.CallbackContext context)
{
    var value = context.ReadValue();
    eventBus.RaiseEvent(h => h.HandleMovement(value));
}
```

Они получают значения ввода и отправляют события дальше.

---
