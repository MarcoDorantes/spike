//cl /EHsc /std:c++20 dt1.cpp

#include <chrono>
#include <iostream>
#include <locale>
#include <sstream>
 
int main()
{
    auto parse = [&](auto str, auto fmt, auto o)
    {
        std::istringstream is{str};
        is.imbue(std::locale("en_US.utf-8"));
        is >> std::chrono::parse(fmt, o);
        is.fail() ? std::cout << "Parse failed!\n" : std::cout << o << '\n';
    };/*
    parse("01:02:03", "%H:%M:%S", std::chrono::hours{});
    parse("01:02:03", "%H:%M:%S", std::chrono::minutes{});
    parse("01:02:03", "%H:%M:%S", std::chrono::seconds{});*/

    parse("13:15:53", "%H:", std::chrono::hours{});
    parse("13:15:53", "%H:%M:", std::chrono::minutes{});
    parse("13:15:53", "%H:%M:%S", std::chrono::seconds{});

//https://learn.microsoft.com/en-us/cpp/standard-library/chrono-functions?view=msvc-180#std-chrono-from-stream
}