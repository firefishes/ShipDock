::设置版本作为目标目录的一部分
set unity_ver=2018_4~

::设置dll, pdb文件名
set dll=%lib_name%.dll
set pdb=%lib_name%.pdb

echo Update %lib_name% dll...

::设置目标路径
set to_path=..\..\ShipDockPlugins\Framework_
set new_path=%to_path%%unity_ver%\

::若目标目录不存在则创建目录
set flag=not exist %new_path%
if %flag% echo Create [%new_path%]
if %flag% md %new_path%

::复制dll文件
set move_from=bin\Debug\%dll%
set copyTo=%move_from% %new_path%

copy %copyTo% >nul

echo Success copy dll file from [%move_from%] to [%new_path%]

::复制pdb文件
set move_from=bin\Debug\%pdb%
set copyTo=%move_from% %new_path%

copy %copyTo% >nul

echo Success copy pdb file from [%move_from%] to [%new_path%]
