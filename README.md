# ProjectManager — Настольное приложение для управления проектами



Простое и удобное WPF-приложение для командной работы над проектами: задачи, участники, комментарии, история изменений.



## Что внутри



* Аутентификация пользователей
* Создание и управление проектами
* Работа с задачами 
* Комментарии к задачам
* Управление участниками проекта 
* История изменений проектов и задач
* Фильтры, поиск и сортировка



## Требования к системе



Операционная система: Windows 10 / 11 (64-bit)
.NET Runtime: .NET 8.0 Desktop Runtime  
СУБД: Microsoft SQL Server 2019 / 2022



## Как установить и запустить



### 1. Установите .NET 8.0 Desktop Runtime



Если ещё не установлен — скачайте и установите:  

→ (https://dotnet.microsoft.com/en-us/download/dotnet/8.0

→ Выберите .NET Desktop Runtime 8.0.x



### 2. Установите и подготовьте SQL Server



* Если у вас ещё нет SQL Server — скачайте SQL Server Express :  

&nbsp; → https://www.microsoft.com/en-us/sql-server/sql-server-downloads



* Запустите установку, выберите Basic или Custom → оставьте Windows Authentication



### 3. Создайте и заполните базу данных



Вам предоставлен готовый SQL-скрипт: ProjectManagerDB.sql



Способ выполнить скрипт :



1\. Откройте SSMS(SQL Server Management Studio)

2\. Подключитесь к серверу `(local)` или `localhost`

3\. Файл → Открыть → Файл… → выберите `ProjectManagerDB.sql`

4\. Нажмите Execute (F5)

5\. Дождитесь сообщения «Command(s) completed successfully»

### 4. Настройте подключение (Необязательно)


* После распаковки приложения вы увидите два основных файла:
  
*ProjectManager.exe
*ProjectManager.exe.config
  

Откройте файл ProjectManager.exe.config в любом текстовом редакторе.

Найдите секцию <connectionStrings> и убедитесь, что строка подключения соответствует вашей ситуации:

<connectionStrings>

   <add name="ProjectManagerDb"

   connectionString="Server=localhost;Database=ProjectManagerDB;Integrated Security=true;TrustServerCertificate=True;"

   providerName="System.Data.SqlClient"/>

</connectionStrings>

### 5. Запустите приложение
Скачайте папку ProjectManagerApp
Внутри будет .exe файл

Просто дважды щёлкните по файлу ProjectManager.exe

При первом запуске вы увидите окно входа.



*Тестовые пользователи:



Логин: Administrator   Пароль: Pass1234
  

### Приятной работы с проектами!



