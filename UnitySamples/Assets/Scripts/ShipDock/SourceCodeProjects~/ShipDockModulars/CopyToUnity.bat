set unity_ver=2018_4~

set dll=%lib_name%.dll
set pdb=%lib_name%.pdb
set to_path=..\..\ShipDockPlugins\Framework_

echo Update %lib_name% dll...
::复制文件
set move_from=bin\Debug\%dll%
set copyTo=%move_from% %to_path%%unity_ver%\
copy %copyTo% >nul

set move_from=bin\Debug\%pdb%
set copyTo=%move_from% %to_path%%unity_ver%\
copy %copyTo% >nul
