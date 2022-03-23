states.uxf - диаграмма состояний и переходов, созданная в программе umlet-standalone-14.2

#первый запуск
Для работы бота необходимо создать файл App.config, содержащий параметры программы. 
На данный момент поддерживаются следующие опции:
- BotToken(является обязательным) - токен бота для подключения к серверам телеграма
- ProxyAddress - настройка прокси сервера, если необходимо
- AdminId - ChatId администратора бота

В качестве образца можно взять файл App.config.example:

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="BotToken" value="PLACE YOUR TELEGRAM BOT TOKEN HERE"/>
    <add key="ProxyAddress" value="195.189.60.23:3128"/>
  </appSettings>
</configuration>

#настройка бота как службы в linux

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

##команды:
sudo cp -r /home/ubuntu/loaded/* /var/SibupkBot   
sudo systemctl restart SibupkBot.service
sudo systemctl start SibupkBot.service
sudo systemctl stop SibupkBot.service

Для включения службы ввести sudo systemctl enable <Имя-службы>

#команды для father bot:

/setname Расписание СибУПК
/setabouttext Расписание СибУПК для студентов и преподавателей
/setdescription Показ расписания СибУПК для студентов очного отделения и преподавателей. Расчет месячной нагрузки для преподавателей.

#команды, доступные пользователю:
help       - инструкция пользователя для бота
settings   - список опций
teachers   - показ преподавателей
fulltime   - показ полного времени 
holiday    - показ выходных дней в расписании

#команды, доступные администраторам бота (доступные так же команды обычного пользователя):
help
showstat        - кол-во запросов расписания
showusers       - показать уникальных пользователей за текущие сутки
updateteachers  - запуск принудительного обновления списка преподавателей
updategroups    - запуск принудительного обновления списка групп
send            - широковещательная рассылка