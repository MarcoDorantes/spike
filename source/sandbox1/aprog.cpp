//cl /EHsc /std:c++20 aprog.cpp
 
#include <iostream>
#include <string>
#include <vector>
#include <list>
#include <algorithm>
#include <ranges>
#include "wnstring.h"
#include "opts.h"

void f1()
{
    for(int n : {0, 1, 2})
    {
        std::cout << " " << n;
	}
    std::cout << std::endl;
}

void f2(int argc, char** argv)
{
    std::cout << "argc: " << argc << std::endl;
    for(int k=0; k<argc; ++k) std::cout << '\t' << argv[k] << std::endl;
}

void f3()
{
    std::vector<int> v = {1,2,3,4,5};
  //for(const int& n : v) std::cout  << '\t' << n;
  //for(auto n : v) std::cout  << '\t' << n;
  //for(auto& n : v) std::cout  << '\t' << n;

    int a[] = {1,2,3,4};
    for(const int& n : a) std::cout  << '\t' << n;
    std::cout << '\n';

    int* b = a;
  //for(const int& n : b) std::cout  << '\t' << n; error C3312: no callable 'begin' function found for type 'int *'
    for(int k=0; k<4;++k) std::cout  << '\t' << *(b+k);
}

void f4(int argc, char** argv)
{
  //std::string s = argv[0];
  //std::cout << s;

    std::vector<std::string> v;
    for(int k=0; k<argc; ++k)
    {
        std::string s = argv[k];
        v.push_back(s);
    }
    std::copy(v.cbegin(), v.cend(), std::ostream_iterator<std::string>(std::cout, "\n"));
}

void f5(int argc, char** argv)
{
  //wn::wnstring s = argv[0];
  //std::cout << s.c_str();

    std::vector<wn::wnstring> v;
    for(int k=0; k<argc; ++k)
    {
      //wn::wnstring s = argv[k]; //https://cppreference.com/w/cpp/language/initialization.html
      //wn::wnstring s(argv[k]);
        wn::wnstring s{argv[k]};
        v.push_back(s);
    }
    for(int k=0; k<v.size(); ++k)
    {
        std::cout << v[k].c_str() << "\n";
    }
  //std::copy(v.cbegin(), v.cend(), std::ostream_iterator<std::string>(std::cout, "\n"));
}

void f6(int argc, char** argv)
{
    wn::opts opt(argc, argv);
  //wn::opts opt = {argc, argv};
    opt.show(std::cout);
  //wn::opts *p = &opt;
  //p->show(std::cout);
}

void f7(int argc, char** argv)
{
    wn::optsptr opt = {argc, argv};
    wn::optsptr *p = &opt;
    p->show(std::cout);
}

void f8(int argc, char** argv)
{
    wn::optslist opt = {argc, argv};
    wn::optslist *p = &opt;
    p->show(std::cout);
}

void f9(int argc, char** argv)
{
    wn::optslist opt = {argc, argv};
    wn::optslist *p = &opt;
    p->print(std::cout);
}

void f10()
{
  auto GG = [](const int n){std::cout << n << "\n";};
  std::vector<int> v = {100,200};
  std::for_each(v.cbegin(),v.cend(),GG);
}

void func3()
{
    std::vector<int> input = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    auto output = input | std::views::filter([](const int n) {return n % 3 == 0; }) | std::views::transform([](const int n) {return n * n; });
    for(const auto& x : output) std::cout << x << "\n";

    std::list<int> input2 = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    auto output2 = input2 | std::views::filter([](const int n) {return n % 3 == 0; }) | std::views::transform([](const int n) {return n * n; });
    for(const auto& x : output2) std::cout << x << "\n";
}

int main(int argc, char** argv)
{
  //std::cout << argc << std::endl;
  //f1();
  //f2(argc, argv);
  //f3();
  //f4(argc, argv);
  //f5(argc, argv);
  //f6(argc, argv);
  //f7(argc, argv);
  //f8(argc, argv);
  //f9(argc, argv);
  //f10();
    func3();
}