# GameJamLvl5
Для художников: https://github.com/CoderLivingDeath/GameJamLvl5/tree/main/Assets/Project/Multimedia/Sprites
Добовлять aseprite файл в эту папку

<img width="435" height="197" alt="image" src="https://github.com/user-attachments/assets/050f609f-1004-443f-a628-080af7caeaac" />

жмякать сюда

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

## Как использовать

1. **Настройка**  
   Создайте и настройте Input Actions в Unity Input System, сгенерируйте класс `InputSystem_Actions`.

2. **Регистрация в Zenject**  
   Свяжите сервисы в вашем Installer-е:

    ```csharp
    Container.Bind().AsSingle();
    Container.Bind().AsSingle().NonLazy();
    Container.Bind>().To().AsSingle();
    ```

3. **Создание InputService**  
   Разрешите и создайте `InputService` с помощью фабрики:

    ```csharp
    var inputServiceFactory = Container.Resolve>();
    InputService inputService = inputServiceFactory.Create();
    ```

4. **Подписка на события**  
   Реализуйте нужные интерфейсы для обработки ввода:

    ```csharp
    public class PlayerMovementHandler : IGameplay_MovementEventHandler
    {
        public void HandleMovement(Vector2 movement)
        {
            // Логика обработки движения игрока
        }
    }
    ```

5. **Регистрируйте подписчиков**  
   Зарегистрируйте обработчики, чтобы EventBus мог их вызывать.

---

## Расширение системы ввода

- Добавьте новые действия в Input System и сгенерируйте обновлённый класс.
- Определите новые константы ключей в `CodeGeneratedInputServiceFactory`.
- Добавьте новые интерфейсы событий, например `INewInput_EventHandler`.
- Подпишитесь на новые действия в `CreateSubscribers` и реализуйте обработчики.
- Реализуйте обработчики в игровых компонентах.

---

## Особенности

- Учитывается фаза `performed` и `canceled` там, где это важно (например, движение).
- Позиция мыши и аналогичные данные считываются как `Vector2`.
- Кнопочные действия передаются как булевы значения (`ReadValueAsButton`).
- Используется событийно-ориентированная архитектура, что упрощает тестирование и декомпозицию.

---

Если у вас есть вопросы, предложения или баги — добро пожаловать в обсуждение и пулл-реквесты!
