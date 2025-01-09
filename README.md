# Top-Down 2D Game Scripts

Набор скриптов для создания 2D Top-Down игры в стиле Hotline Miami.

## Описание скриптов

### PlayerController.cs
Скрипт управления игроком:
- Передвижение на WASD
- Бег при нажатии Shift
- Поворот персонажа за курсором мыши
- Плавное движение с использованием Rigidbody2D

### DoorController.cs
Скрипт для дверей:
- Автоматическое открытие при приближении игрока
- Определение стороны, с которой подходит игрок
- Плавное открытие/закрытие
- Настраиваемые параметры открытия

### FireSystem.cs
распространение огня:
- Настраиваемый интервал появления огня
- Постепенный рост эффекта огня
- Нанесение урона игроку в зоне действия
- Визуальные эффекты

### StudentAI.cs
ИИ студентов:
- Случайное перемещение по уровню
- Следование за игроком при нажатии E
- Избегание препятствий
- Настраиваемые параметры движения

### LevelExitZone.cs
выход из уровня:
- Переход в меню при входе игрока
- Опциональная проверка всех спасенных студентов
- Настраиваемая задержка перехода

### PlayerHealth.cs
 здоровье игрока:
- Система урона и лечения
- Автоматическая регенерация здоровья
- События для UI и эффектов
- Система смерти

## Инструкция по настройке

### Настройка игрока
1. объект игрока и добавь компоненты:
   - PlayerController
   - PlayerHealth
   - Rigidbody2D (с параметром Freeze Rotation)
   - Collider2D

### Настройка дверей
1. объекты с компонентами:
   - DoorController
   - Rigidbody2D
   - BoxCollider2D

### Настройка системы огня
1. Создай объекты с:
   - FireSystem
   - Подготовь префаб эффекта огня

### Настройка студентов
1. Создай префаб с компонентами:
   - StudentAI
   - Rigidbody2D
   - Collider2D

### Настройка зоны выхода
1. Создай объект с:
   - LevelExitZone
   - BoxCollider2D (Is Trigger = true)

FireExtinguisher.cs:
Создает частицы пены
Тушит огонь при попадании
ExtinguisherFoam.cs:
Управляет частицами пены
Пена появляется, расширяется и исчезает за 3 секунды
3. Модификация FireSystem.cs:
Огонь полностью прекращает работу при тушении


Чтобы использовать  скрипты пожаротушения:
Создай префаб пены с компонентами:
SpriteRenderer

ExtinguisherFoam

На объект игрока добавь крч:
FireExtinguisher

Настрой точку спрея (sprayPoint)

Присвой префаб пены
Настрой слой для огня
