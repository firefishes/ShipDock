
echo Will create hot fix: %name%
echo Start rename...

::定义dll和pdb文件名
set dll_file=%release%%name%.dll
set pdb_file=%release%%name%.pdb

::保存当前目录路径
set root=%~dp0

::定义需要重命名的文件名
set rn_dll=%root%%dll_file%
set rn_pdb=%root%%pdb_file%

::定义扩展名
set s=bytes

ren %rn_dll% %name%.d.%s%
ren %rn_pdb% %name%.p.%s%

::定义即将移动到的Unity工程路径
set move_to=..\res_data\%hotfix_ab_name%\
set move_to_init=..\Resources\

echo Start move file...

::移动热更文件
echo Will release hotfix file to %move_to_init% folder..

copy %root%%release%%name%.d.%s% %move_to_init%
copy %root%%release%%name%.p.%s% %move_to_init%

echo Will release hotfix file to %move_to% folder..

move %root%%release%%name%.d.%s% %move_to%
move %root%%release%%name%.p.%s% %move_to%

echo Finished..
pause