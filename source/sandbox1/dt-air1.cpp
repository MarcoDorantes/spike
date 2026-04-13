#include <iostream>
#include <sstream>
#include <chrono>
#include <locale>

void TestFlags( std::ios& x )
{
    std::cout << ( x.rdstate() & std::ios::badbit ) << std::endl;
    std::cout << ( x.rdstate() & std::ios::failbit ) << std::endl;
    std::cout << ( x.rdstate() & std::ios::eofbit ) << std::endl;
    std::cout << std::endl;
}
int main() {//1. Parse into std::chrono::seconds duration, then decompose:
  //std::string input = "13:15:53";
  //std::string input = "02:15:53";
  //std::string input = "12";
    std::string input = "13:15";
    std::istringstream iss{input};
  //iss.imbue(std::locale("en_US.utf-8"));

  //std::chrono::seconds total{};
    std::chrono::minutes total{};
  //std::chrono::hours total{};
  //iss >> std::chrono::parse("%H:%M:%S", total);
  //iss >> std::chrono::parse("%M", total);
    iss >> std::chrono::parse("%H:%M", total);

    if (iss.fail()) {
        std::cerr << "Parse failed.\n";
        std::cerr << iss.rdstate() << std::endl;
        TestFlags(iss);
        return 1;
    }
    else std::cout << total << '\n';/*
    auto hours   = std::chrono::duration_cast<std::chrono::hours>(total);
    auto minutes = std::chrono::duration_cast<std::chrono::minutes>(total % std::chrono::hours{1});
    auto seconds = total % std::chrono::minutes{1};

    std::cout << "Total seconds : " << total.count()   << '\n'; // 47753
    std::cout << "Hours         : " << hours.count()   << '\n'; // 13
    std::cout << "Minutes       : " << minutes.count() << '\n'; // 15
    std::cout << "Seconds       : " << seconds.count() << '\n'; // 53
*/
}