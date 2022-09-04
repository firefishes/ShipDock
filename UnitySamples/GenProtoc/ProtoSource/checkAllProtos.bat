
for /f "delims=" %%i in ('dir /b *.proto') do (
	type %%i>>%file_temp%
	echo %%i)

cd /d %~dp0
