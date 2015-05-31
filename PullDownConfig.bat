git clone %1 temp
xcopy temp\* . /s /i
rm -rf temp