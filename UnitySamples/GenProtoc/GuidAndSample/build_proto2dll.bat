rem 将 msg.proto, msgdb.proto合并成一个临时文件temp.proto
type msg.proto>temp.proto
type msgdb.proto>>temp.proto

@echo off
rem proto生成cs文件 命令窗口可切换到protogen.exe所在目录,.\protogen 查看帮助文档
Protogen\protogen.exe -i:.\temp.proto -o:ProtoMessage.cs -p:detectMissing

rem 删除临时文件
del temp.proto

rem CD表示当前执行的目录
set base_dir=%CD%
set dll_Name=ProtoMessage.dll

rem 拷贝cs文件,做备份
xcopy %base_dir%\*.cs  %base_dir%\..\ProtoBufLib\ /y

rem 获取proto编译器路径
set precomile=\Precompile\precompile.exe
rem csc.exe是C#编译器,将cs生成dll
set csc_make=C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe

rem 将cs文件生成dll文件,/r:引用资源,将protobuf-net.dll并作为引用文件 /out:输出dll名称 /target:library 库文件类型
%csc_make% /r:%base_dir%\unity\protobuf-net.dll /out:%dll_Name% /target:library *.cs

rem 给生成dll附加序列化和反序列化相关代码, -p:含有序列化和反序列化相关代码的附加目录 -t:设置生成dll的命名空间
%base_dir%%precomile% %dll_Name% -o:Serializer_%dll_Name% -p:%base_dir%\unity\ -t:ProtobufSerializer

rem 拷贝到unity工程的plugins文件夹下
rem xcopy %base_dir%\*.cs  %base_dir%\..\ProtoBufLib\ /y
pause