@echo off

call HotFixSettings.bat

::定义即将拷贝的dll热更文件所在目录
set release=..\..\Temp\Bin\Release\

::执行拷贝
call CopyHotFix.bat
