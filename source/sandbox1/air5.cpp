//cl /EHsc /std:c++20 /Iinclude air5.cpp

//Generalizing with a custom ostream:

#include <iostream>
#include <vector>
#include <algorithm>
#include <string>
#include <nlohmann/json.hpp>

template <typename Container>
void print_json(const Container& c, std::ostream& os = std::cout, int indent = -1) {
    nlohmann::json j = nlohmann::json::array();

    std::for_each(c.begin(), c.end(), [&](const auto& elem) {
        j.push_back(elem);
    });

    os << j.dump(indent) << '\n';
}

int main() {
    std::vector<int>         nums  = {10, 20, 30};
    std::vector<std::string> words = {"hello", "world", "C++20"};

    print_json(nums);              // Output: [10,20,30]
    print_json(nums,  std::cout, 2); // Pretty-printed
    print_json(words, std::cerr);  // Output to stderr: ["hello","world","C++20"]
    print_json(words, std::cout, 2); // Pretty-printed
}