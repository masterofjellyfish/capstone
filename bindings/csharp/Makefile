all:
	mkdir bin
	mcs Capstone/capstone.cs Capstone/capstone_const.cs Capstone/arm*.cs Capstone/mips*.cs Capstone/x86*.cs Capstone/ppc*.cs -t:library -d:MONO -out:bin/capstone.dll
	mcs Test/test.cs -r:capstone.dll -lib:bin/ -out:bin/test.exe
	mcs Test_PowerPC/test_ppc.cs -r:capstone.dll -lib:bin/ -out:bin/test_ppc.exe
	mcs Test_X86/test_x86.cs -r:capstone.dll -lib:bin/ -out:bin/test_x86.exe
	mcs Test_Mips/test_mips.cs -r:capstone.dll -lib:bin/ -out:bin/test_mips.exe
	mcs Test_Arm/test_arm.cs -r:capstone.dll -lib:bin/ -out:bin/test_arm.exe
	mcs Test_Arm64/test_arm64.cs -r:capstone.dll -lib:bin/ -out:bin/test_arm64.exe
	chmod +x bin/*.exe

clean:
	rm -rf lib
	rm -rf bin

