#include <iostream>
#include <sstream>
#include <chrono>

int main() {//2. Using std::chrono::hh_mm_ss to decompose directly:
    std::string input = "13:15:53";
    std::istringstream iss{input};

    std::chrono::seconds total{};
    iss >> std::chrono::parse("%H:%M:%S", total);

    if (iss.fail()) {
        std::cerr << "Parse failed.\n";
        return 1;
    }

    // hh_mm_ss splits duration into hours/minutes/seconds components
    std::chrono::hh_mm_ss time{total};

    std::cout << "Total seconds : " << total.count()              << '\n'; // 47753
    std::cout << "Hours         : " << time.hours().count()       << '\n'; // 13
    std::cout << "Minutes       : " << time.minutes().count()     << '\n'; // 15
    std::cout << "Seconds       : " << time.seconds().count()     << '\n'; // 53
}