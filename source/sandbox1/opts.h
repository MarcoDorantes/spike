#pragma once

#include <iostream>
#include <string>
#include <vector>
#include <list>
#include <algorithm>
#include "wnstring.h"

//      void FF(const wn::wnstring& s) { std::cout << s.c_str() << "\n"; };

namespace wn
{
    class opts
    {
        std::vector<wn::wnstring> v;

    public:
        opts(int argc, char** argv)
        {
            v.reserve(argc);
            for(int k=0; k<argc; ++k)
            {
                wn::wnstring s{argv[k]};
                v.push_back(s);

              //v.push_back(wn::wnstring(argv[k]));
              //v.push_back(wn::wnstring{argv[k]});
            }
        }

        void show(std::ostream& out)
        {
            for(auto& x : v)
            {
                out << x.c_str() << "\n";
            }
        }
    };

    class optsptr
    {
        std::vector<const char*> v;

    public:
        optsptr(int argc, char** argv)
        {
            for(int k=0; k<argc; ++k)
            {
                v.push_back(argv[k]);
            }
        }

        void show(std::ostream& out)
        {
            for(int k=0; k<v.size(); ++k)
            {
                out << v[k] << "\n";
            }
        }
    };

    class optslist
    {
        std::list<wn::wnstring> v;

    public:
        optslist(int argc, char** argv)
        {
            for(int k=0; k<argc; ++k)
            {
                wn::wnstring s{argv[k]};
                v.push_back(s);

              //v.push_back(wn::wnstring(argv[k]));
              //v.push_back(wn::wnstring{argv[k]});
            }
        }

        void show(std::ostream& out)
        {
            for(auto& x : v)
            {
                out << x.c_str() << "\n";
            }
        }

        void print(std::ostream& out)
        {
            auto display = [&](const wn::wnstring& s) { out << "s.c_str()" << "\n"; };
            std::for_each(v.cbegin(), v.cend(), display);

          //std::for_each(v.cbegin(), v.cend(), FF);
        }
    };
}