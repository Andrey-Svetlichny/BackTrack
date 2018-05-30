@echo off

rmdir /S /Q .vs

for /f %%i in ('dir /a:d /s /b ^| findstr /i /e "bin"') do rmdir /S /Q %%i
for /f %%i in ('dir /a:d /s /b ^| findstr /i /e "obj"') do rmdir /S /Q %%i

