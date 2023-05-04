# SaberInteractiveTestTask
 
Задание 1

 Реализуйте функции сериализации и десериализации двусвязного списка, заданного следующим образом:
 
    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }


    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
        }

        public void Deserialize(FileStream s)
        {
        }
    }
    
    Задание 2 
    Напишите ИИ для противника используя BhvTree (достаточно нарисовать схему, реализация в каком-либо из движков не требуется).
Солдат - сущность, которая может стрелять, отправиться в указанную точку и ждать.
Солдат проводит патруль по зацикленному маршруту по точкам А и Б
По прибытии на точку солдат останавливается на 5 минут в ожидании врага. Если за 5 минут враг не появился, солдат идет на следующую точку.
Если враг обнаружен, солдат стреляет в него с паузой в 1 секунду между каждым выстрелом
![Screenshot_1](https://user-images.githubusercontent.com/22577093/236282002-d714eafc-e6cf-4f28-86df-55a2eb132c14.png)

Узлы
Is enemy nearby
Проверка на наличие врага по близости

success - враг найден
failure - враг не найден

Shoot Enemy
Стрельба по противнику
running - производится стрельба по противнику

Wait(1s) - ожидание после выстрела
running - задержка на 1 секунду

Am I still waiting?
Проверка на конец ожидания на патрульной точке
success - прошло 5 минут ожидания
failure - 5 минут еще не прошло

Wait
Ожидание на точке патрулирования
running - задержка на фрейм/1 секунду, продвижение таймера ожидания

Walk forward 
Продвижение к точке патрулирования
running - передвижение к точке

Am I on Spot?
Проверка прибытия на точку
success - персонаж на точке ожидания
failure - персонаж еще в пути

Change destination
Меняет точку, к которой нужно идти
running - точка меняется с А на Б или с Б на А

Start waiting timer
Устанавливает таймер ожидания
running - ставит таймер на 5 минут

