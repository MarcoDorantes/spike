#include <iostream>
#include <sstream>
#include <chrono>

int main() {//3. Using std::chrono::from_stream — the lower-level alternative to parse:
    std::string input = "13:15:53";
    std::istringstream iss{input};

    std::chrono::seconds total{};
    std::chrono::from_stream(iss, "%H:%M:%S", total);

    if (iss.fail()) {
        std::cerr << "Parse failed.\n";
        return 1;
    }

    std::chrono::hh_mm_ss time{total};

    std::cout << "Total seconds : " << total.count()          << '\n'; // 47753
    std::cout << "Hours         : " << time.hours().count()   << '\n'; // 13
    std::cout << "Minutes       : " << time.minutes().count() << '\n'; // 15
    std::cout << "Seconds       : " << time.seconds().count() << '\n'; // 53
}