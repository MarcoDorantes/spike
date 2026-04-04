#pragma once

#include <iostream>
#include <string>
#include <vector>
#include <list>
#include <algorithm>
#include "wnstring.h"

void F1(wn::wnstring& s) { std::cout << s.c_str() << "\n"; };
void F2(const wn::wnstring& s) { std::cout << s.c_str() << "\n"; };

void F3(auto& s) { std::cout << s.c_str() << "\n"; };
template<typename T> void F4(T& t) { std::cout << t.c_str() << "\n"; };

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

        void print0(std::ostream& out)
        {
            auto display1 = [&](const wn::wnstring& s) { out << s.c_str() << "\n"; };
            std::for_each(v.cbegin(), v.cend(), display1);

            auto display2 = [&out](wn::wnstring& s) { out << s.c_str() << "\n"; };
            std::for_each(v.begin(), v.end(), display2);

            std::for_each(v.begin(), v.end(), F1);
            std::for_each(v.cbegin(), v.cend(), F2);

            std::ranges::for_each(v, [&out](const wn::wnstring& s) { out << s.c_str() << "\n"; });
            std::ranges::for_each(v, [&out](wn::wnstring& s) { out << s.c_str() << "\n"; });
            std::ranges::for_each(v, display1);
            std::ranges::for_each(v, display2);

            wn::wnstring& s1 = v.back();
            F3(s1);
            F4(s1);

            const wn::wnstring& s2 = v.back();
            F3(s2);
            F4(s2);

            auto& s3 = v.back();
            F3(s3);
            F4(s3);
        }

        void print1(std::ostream& out)
        {
            auto r = std::ranges::all_of(v, [](const wn::wnstring& s) { return s.c_str() != nullptr; });
            out << (r ? "OK" : "Invalid") << "\n";

            std::list<std::string> _v;
            std::ranges::for_each(v, [&_v](const wn::wnstring& s) { _v.push_back(std::string(s.c_str())); });
            const std::string x{"A"};
            if(auto f = std::ranges::find(_v, x); f != _v.end())
            {
                out << "FOUND " << x << "\n";
            }
            else out << "NOT FOUND " << x << "\n";
        }
    };
}