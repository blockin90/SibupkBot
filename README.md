#SibupkBot
Телеграм бот для просмотра расписания сибупк

#первый запуск

Бот реализован в виде конечного автомата, диаграмму состояний см.в проект TelegramBotClient/docs/

##команды для father bot:
/setname Расписание СибУПК
/setabouttext Расписание СибУПК для студентов и преподавателей
/setdescription Показ расписания СибУПК для студентов очного отделения и преподавателей. Расчет месячной нагрузки для преподавателей.

##Настройка параметров бота
Для работы бота необходимо создать файл App.config, содержащий параметры программы. 
На данный момент поддерживаются следующие опции:
- BotToken(является обязательным) - токен бота для подключения к серверам телеграма
- ProxyAddress - настройка прокси сервера, если необходимо
- AdminId - ChatId администратора бота

В качестве образца можно взять файл App.config.example:

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="AdminId" value="0123456789"/>
    <add key="BotToken" value="PLACE YOUR TELEGRAM BOT TOKEN HERE"/>
    <add key="ProxyAddress" value="195.189.60.23:3128"/>
  </appSettings>
</configuration>

##Настройка параметров бота для новой версии (проект SibupkScheduleBot)
Для работы бота необходимо создать файл appsettings.json, содержащий параметры программы. 
На данный момент поддерживаются следующие опции:
- BotToken(является обязательным) - токен бота для подключения к серверам телеграма

В качестве образца можно взять файл appsettings.sample

##настройка бота как службы в linux

разворачивание бота на линуксе как службу: https://developers.redhat.com/blog/2017/06/07/writing-a-linux-daemon-in-c/
содержимое файла /etc/systemd/system/SibupkBot.service:

[Unit]
Description=Telegram bot for SibUPK schedule
DefaultDependencies=no

[Install]
WantedBy=multi-user.target

[Service]
Type=simple
ExecStart=/usr/share/dotnet/dotnet UpkTelegramClient.dll
WorkingDirectory=/var/SibupkBot
User=dotnetuser
Group=dotnetuser
Restart=always
RestartSec=10


#команды, доступные пользователю:
help       - инструкция пользователя для бота
settings   - список опций
teachers   - показ преподавателей
fulltime   - показ полного времени 
holiday    - показ выходных дней в расписании

#команды, доступные администраторам бота (также доступны команды обычного пользователя):
help
showstat        - кол-во запросов расписания
showusers       - показать уникальных пользователей за текущие сутки
updateteachers  - запуск принудительного обновления списка преподавателей
updategroups    - запуск принудительного обновления списка групп
send            - широковещательная рассылка