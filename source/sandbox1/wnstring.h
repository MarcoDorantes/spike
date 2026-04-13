#pragma once

//#include <string>

namespace wn
{
    class wnstring //?: public std::string
    {
        const char* p;
        char ctor;
      //std::string

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

        // Simple move constructor ??
        wnstring(wnstring&& x) : p(std::move(x.p)), ctor('C') {}
 
        // Simple move assignment operator ??
        wnstring& operator=(wnstring&& other)
        {
            p = std::move(other.p);
            ctor = 'C';
            return *this;
        }

        ~wnstring()
        {
            std::cout << "\t["<<ctor<<"] ~wnstring("<<p<<")\n";
        }

        const char* c_str() const { return p; }
    };
}