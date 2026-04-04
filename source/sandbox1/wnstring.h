#pragma once

//#include <string>

namespace wn
{
    class wnstring //?: public std::string
    {
        const char* p;
        char ctor;

    public:
        wnstring(const char* s) : p(s)
        {
            ctor = 'A';
            std::cout << "\twnstring(const char* "<<s<<")\n";
        }

        wnstring(const wnstring& other) : p(other.p)
        {
            ctor = 'B';
            std::cout << "\twnstring(const wnstring& "<<p<<")\n";
        }

        ~wnstring()
        {
            std::cout << "\t["<<ctor<<"] ~wnstring("<<p<<")\n";
        }

        const char* c_str() const { return p; }
    };
}