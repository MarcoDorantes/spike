//cl /EHsc /std:c++20 /Iinclude air4.cpp

#include <iostream>
#include <vector>
#include <algorithm>
#include <nlohmann/json.hpp>

int main() {
    std::vector<int> nums = {1, 2, 3, 4, 5};

    nlohmann::json j = nlohmann::json::array();

    std::for_each(nums.begin(), nums.end(), [&](const auto& elem) {
        j.push_back(elem);
    });

    std::cout << j.dump() << '\n';
    // Output: [1,2,3,4,5]

    std::cout << j.dump(2) << '\n';
    // Pretty output:
    // [
    //   1,
    //   2,
    //   3,
    //   4,
    //   5
    // ]
}