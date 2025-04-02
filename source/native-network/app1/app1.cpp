// app1.cpp : This file contains the 'main' function. Program execution begins and ends there.
// msbuild .\source\native-network\app1\ -p:Configuration=Release

#include <iostream>

class XX
{
    int ID;

    int getid(int n) { if (n < 0) throw std::exception("Invalid ID"); return n; }

public:
    XX(int n):ID(getid(n)) {}

    void show(std::ostream& o)
    {
        o << "ID = " << ID << std::endl;
    }
};

int main()
{
    std::cout << "Hello World!\n";
    try
    {
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
