//cl /EHsc /std:c++20 /Iinclude air6.cpp

//C++20 std::ranges::for_each version:

#include <iostream>
#include <vector>
#include <algorithm>
#include <string>
#include <nlohmann/json.hpp>

int main() {
    std::vector<std::string> words = {"hello", "world", "C++20"};

    nlohmann::json j = nlohmann::json::array();//https://json.nlohmann.me/api/basic_json/array/#notes

    std::ranges::for_each(words, [&](const auto& elem) {
        j.push_back(elem);
    });

    // Compact
    std::cout << j.dump() << '\n';
    // Output: ["hello","world","C++20"]

    // Pretty-printed with 2-space indent
    std::cout << j.dump(2) << '\n';
    // Output:
    // [
    //   "hello",
    //   "world",
    //   "C++20"
    // ]
}/*
nlohmann::json::array() explicitly creates a JSON array node, keeping intent clear.
j.push_back(elem) works for any type the library supports (int, double, std::string, bool, nested containers, etc.) — no if constexpr type-juggling needed.
j.dump() produces a compact, spec-compliant JSON string; j.dump(2) pretty-prints it with a 2-space indent — the library handles all quoting, escaping, and comma placement correctly.
The library is available via vcpkg, Conan, or as a single header drop-in from github.com/nlohmann/json.
*/