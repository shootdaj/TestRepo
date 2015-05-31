git clone %1 temp
xcopy temp\* . /s /i /y
rm -rf temp