// app1.cpp : This file contains the 'main' function. Program execution begins and ends there.
// msbuild .\source\native-network\app1\ -p:Configuration=Release

#include <iostream>

class YY
{
    int ID;
public:
    YY() :ID(-1) { std::cout << "YY:ctor()\n"; }
    YY(int n) {}
    YY(const YY& other):ID(other.ID) { std::cout << "YY:ctor(YY)\n"; }
//  YY(const YY& other) = delete; //Copy not allowed.
};

class XX
{
    int ID;
    YY y;

    int getid(int n)
    {
        std::cout << "XX:ctor("<<n<<")\n";
        if (n < 0) throw std::exception("Invalid ID");
        return n;
    }

public:
    XX():ID(-1) { std::cout << "XX:ctor()\n"; }
    XX(int n):ID(getid(n)) {}
//  XX(const XX& other):ID(other.ID) { std::cout << "XX:ctor(XX)\n"; }
//  XX(const XX& other) = delete; //Copy not allowed.

    void show(std::ostream& o)
    {
        o << "ID = " << ID << std::endl;
    }
};

struct point {
    int x;
    int y;
};

void fpoint(struct point p)
{
    printf("%d %d\n", p.x, p.y);
    p.x = 2;
    p.y = 3;
}

void passStructByVal()
{
    struct point pxx = { 100,200 };
    fpoint(pxx);
    printf("%d %d\n", pxx.x, pxx.y);
}

void fXX(XX aa)
{
    aa.show(std::cout);
}

void passClassByVal()
{
    XX a{ 10 };
    fXX(a);
    a.show(std::cout);
}

int main()
{
    std::cout << "Hello World!\n";
    try
    {
        passStructByVal();
        passClassByVal();

        XX x{ -1 };
        x.show(std::cout);
    }
    catch (const std::exception& ex) //https://www.drdobbs.com/when-and-how-to-use-exceptions/184401836
    {
        std::cout << ex.what();
    }
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
