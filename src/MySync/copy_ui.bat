del ../../bin/ui

robocopy .\..\src\MySync\ui %1%\..\..\bin\ui /s > buildlog.txt
exit 0