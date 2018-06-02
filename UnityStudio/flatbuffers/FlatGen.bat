setlocal enableextensions enabledelayedexpansion

::协议Schema路径
set protocolPath=protocol/
::协议Schema扩展名
set protocolEx=.fbs
::协议编译器路径
set protocolCompilerPath=flatc
::协议编译器公共参数
set protocolCompilerArg=--gen-onefile
::Mono.Net编译器路径
set MonoCompilerPath=Mono-2.11.4\lib\mono\4.5\mcs.exe
::Mono.Net编译公共参数
set MonoCompilerArg=-t:library -optimize -sdk:2 -d:UNSAFE_BYTEBUFFER
::输出项目路径
set projectDir=..\Assets\1Party\FlatBuffers\Plugins\

::处理输出临时文件夹
mkdir output
del output\*.cs /q
del output\FlatBuffersModel.dll /q

::生成Flat协议代码
set temp=
for %%i in (%protocolPath%*%protocolEx%) do set temp=!temp! %protocolPath%%%i
%protocolCompilerPath% %protocolCompilerArg% -o output -n %temp%

::由于.net与mono之间支持标准略有差异,所以协议代码由Mono编译
%MonoCompilerPath% %MonoCompilerArg% -out:output\FlatBuffersModel.dll -r:output\FlatBuffers.dll output\*.cs

::输出到项目
COPY output\FlatBuffersModel.dll %projectDir%

@pause