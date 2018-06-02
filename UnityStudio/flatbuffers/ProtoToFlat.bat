setlocal enableextensions enabledelayedexpansion

::FlatBuffers路径
set protocolDir=protocol/
::输出项目路径
set outDir=protocol/

::编译协议
set temp=
for %%i in (%protocolDir%*.proto) do flatc.exe --gen-onefile -o %outDir% --proto %protocolDir%%%i


@pause