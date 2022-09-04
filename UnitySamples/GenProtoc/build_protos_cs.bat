@echo off

::最终CS类文件名生成的目录名
set scripts_dir_name=PeaceGame

::定义生成器位置
set protogen_tool=Protogen\protogen.exe
::定义协议文件位置
set source=ProtoSource\
::定义合并后的CS类文件名
set file_protos=Protos.cs

::临时文件名
set file_temp=merged.proto

::打开协议文件目录遍历所有协议文件并合并
cd /d %~dp0%source%
call checkAllProtos.bat
cd /d %~dp0

::生成protobuff类文件和序列化后的文件
%protogen_tool% -i:.\%source%%file_temp% -o:%file_protos% -p:detectMissing

::删除合并所有协议后的临时文件
del .\%source%%file_temp%

pause