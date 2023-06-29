//cl /EHsc .\prog1.cpp

#include <iostream>
#include <locale>

#include <clocale>
#include <cstdio>
#include <ctime>
#include <cwchar>
#include <iterator>
#include <string>
 
void loc1()//https://en.cppreference.com/w/cpp/locale/setlocale
{
    // Make a "deep copy" of current locale name.
    std::string prev_loc = std::setlocale(LC_ALL, nullptr);
 
    // The C locale will be UTF-8 enabled English,
    // decimal dot will be German,
    // date and time formatting will be Japanese.
    if (const char* loc = std::setlocale(LC_ALL, "en_US.UTF-8"))
        std::cout << "New LC_ALL locale: " << loc << std::endl;//std::wprintf(L"New LC_ALL locale: %s\n", loc);
    if (const char* loc = std::setlocale(LC_NUMERIC, "de_DE.UTF-8"))
        std::cout << "New LC_NUMERIC locale: " << loc << std::endl;//std::wprintf(L"New LC_NUMERIC locale: %s\n", loc);
    if (const char* loc = std::setlocale(LC_TIME, "ja_JP.UTF-8"))
        std::cout << "New LC_TIME locale: " << loc << std::endl;//std::wprintf(L"New LC_TIME locale: %s\n", loc);
 
    /*wchar_t buf[100];
    std::time_t t = std::time(nullptr);
    std::wcsftime(buf, std::size(buf), L"%A %c", std::localtime(&t));
    std::wprintf(L"Number: %.2f\nDate: %Ls\n", 3.14, buf);
*/
    // Restore the previous locale.
    if (const char* loc = std::setlocale(LC_ALL, prev_loc.c_str()))
        std::cout << "Restored LC_ALL locale: " << loc << std::endl;//std::wprintf(L"Restorred LC_ALL locale: %s\n", loc);
}

void loc2()//https://learn.microsoft.com/en-us/cpp/c-runtime-library/reference/setlocale-wsetlocale?view=msvc-170
{
    std::string prev_loc = std::setlocale(LC_ALL, nullptr);

    if (const char* loc = std::setlocale(LC_ALL, ".1252"))
        std::cout << "New LC_ALL locale: " << loc << std::endl;

    if (const char* loc = std::setlocale(LC_ALL, "English_United States.1252"))
        std::cout << "New LC_ALL locale: " << loc << std::endl;
    
    if (const char* loc = std::setlocale(LC_ALL, prev_loc.c_str()))
        std::cout << "Restored LC_ALL locale: " << loc << std::endl;
}

void globalized()//https://en.cppreference.com/w/cpp/locale/locale
{
    std::wcout << "User-preferred locale setting is "
               << std::locale("").name().c_str() << '\n';
    // on startup, the global locale is the "C" locale
    std::wcout << 1000.01 << '\n';
 
    // replace the C++ global locale and the "C" locale with the user-preferred locale
    std::locale::global(std::locale(""));
    // use the new global locale for future wide character output
    std::wcout.imbue(std::locale());
 
    // output the same number again
    std::wcout << 1000.01 << '\n';
}

int main() {
  std::cout << "Hello World!\n";

  globalized();
  loc1();
  loc2();

  return 0;
}