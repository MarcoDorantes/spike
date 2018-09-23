//cl /EHsc /std:c++17 fs1.cpp

#include <iostream>
#include <filesystem>

namespace _fs=std::filesystem;

int main(int argc, char** argv)
{
try
{
/*
https://channel9.msdn.com/Events/Build/2014/2-661
https://en.cppreference.com/w/cpp/filesystem
https://youtu.be/7LjIMraEBuU?t=378
*/
	_fs::path current = _fs::current_path();
	std::cout << "here - current path: " << current << std::endl;
	for(auto& x: _fs::directory_iterator(current))
	{
		auto what = _fs::is_regular_file(x) ? "file" : (_fs::is_directory(x)? "folder" : "unknown");
		std::cout << "\t" << what << "\t" << x << std::endl;
	}
}
catch(std::exception& ex){std::cout << ex.what() << std::endl;}
}