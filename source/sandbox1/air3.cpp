#include <iostream>
#include <vector>
#include <algorithm>  // std::ranges::for_each

int main() {
    std::vector<std::string> words = {"hello", "world", "C++20"};

    // Cleaner: pass container directly, no .begin()/.end() needed
    std::ranges::for_each(words, [](const auto& elem) {
        std::cout << elem << '\n';
    });
}/*
C++20 range-based alternative with std::ranges::for_each:
The C++20 std::ranges::for_each is the most idiomatic option — it accepts the container directly (no need for .begin()/.end()), works with any range, and pairs cleanly with a lambda.
*/