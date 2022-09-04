@echo off

call build_protos_cs.bat

::定义变量当前执行的目录
set base_dir=%CD%
::定义协议dll文件名
set name_dll=Protos.dll
::定义序列化后的协议dll文件名
set name_serializer_dll=Protos.dll

::拷贝cs类文件做备份
xcopy %base_dir%\*.cs  %base_dir%\..\ProtoBufLib\ /y

::获取proto编译器路径
set precomile=\Precompile\precompile.exe
::使用C#编译器将cs类生成dll
set csc_make=C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe

::将cs文件生成dll文件，
::参数/r - 引用资源，将protobuf-net.dll并作为引用文件
::参数/out - 输出dll名称
::参数/target - library 库文件类型
%csc_make% /r:%base_dir%\unity\protobuf-net.dll /out:%name_dll% /target:library *.cs

::给生成的dll附加序列化和反序列化相关代码
::参数-p - 含有序列化和反序列化相关代码的附加目录
::参数-t - 设置生成dll的命名空间
%base_dir%%precomile% %name_dll% -o:Serializer_%name_dll% -p:%base_dir%\unity\ -t:ProtobufSerializer

::复制dll文件
set new_path=..\Assets\OtherPlugins\Protos\

set move_from=Serializer_%name_dll%
set copy_to=%move_from% %new_path%
copy %copy_to% >nul

set move_from=%name_dll%
set copy_to=%move_from% %new_path%
copy %copy_to% >nul

pause