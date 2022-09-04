@echo off

call build_protos_cs.bat

::定义变量当前执行的目录
set base_dir=%CD%
::定义协议dll文件名
set name_dll=Protos.dll
::定义序列化协议dll文件名
set name_serializer_dll=Serializer_Protos.dll

::拷贝cs类文件做备份
xcopy %base_dir%\*.cs  %base_dir%\..\ProtoBufLib\ /y

::获取proto编译器路径
set precomile=\Precompile\precompile.exe
::使用C#编译器将cs类生成dll
set csc_make=C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe

::将cs文件生成dll文件，
%csc_make% /r:%base_dir%\unity\protobuf-net.dll /out:%name_dll% /target:library *.cs

::给生成的dll附加序列化和反序列化相关代码
%base_dir%%precomile% %name_dll% -o:%name_serializer_dll% -p:%base_dir%\unity\ -t:ProtobufSerializer

::若目标目录不存在则创建目录
set new_path=..\Assets\OtherPlugins\Protos\
set flag=not exist %new_path%
if %flag% echo Create [%new_path%]
if %flag% md %new_path%

::复制dll文件
set move_from=.\%name_serializer_dll%
set copy_to=%move_from% %new_path%
copy %copy_to% >nul

set move_from=.\%name_dll%
set copy_to=%move_from% %new_path%
copy %copy_to% >nul

::删除合并所有协议后的临时文件
del .\%file_protos%
del .\%name_dll%
del .\%name_serializer_dll%

pause