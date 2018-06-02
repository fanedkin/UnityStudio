setlocal enableextensions enabledelayedexpansion

::FlatBuffers库代码路径
set flatbuffersDir=flatbuffers-1.7.1\net\FlatBuffers\
::VS.Net编译器路径
set VSCompilerPath=C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe
::VS.Net编译公共参数
set VSCompilerArg= -t:library /optimize /unsafe /define:UNSAFE_BYTEBUFFER
::输出项目路径
set projectDir=..\Assets\1Party\FlatBuffers\Plugins\

::处理输出临时文件夹
mkdir output
del output\*.* /q

::.Net编译FlatBuffers库
%VSCompilerPath% %VSCompilerArg% /out:output\FlatBuffers.dll %flatbuffersDir%*.cs

::输出到项目
COPY output\FlatBuffers.dll %projectDir%

@pause