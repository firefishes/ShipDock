rem 将 msg.proto, msgdb.proto合并成一个临时文件temp.proto
type msg.proto>temp.proto
type msgdb.proto>>temp.proto

@echo off
rem 执行protogen.exe,进行转化,了解更多进入protogen.exe所在目录,
rem Shift+鼠标右键打开PowerShell窗口,输入:.\protogen 查看帮助文档
Protogen\protogen.exe -i:.\temp.proto -o:ProtoMessage.cs -p:detectMissing

rem 删除临时文件
del temp.proto

pause
