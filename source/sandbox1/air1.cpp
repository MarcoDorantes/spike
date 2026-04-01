#include <iostream>
#include <vector>
#include <algorithm>

int main() {
    std::vector<int> nums = {1, 2, 3, 4, 5};

    // Using std::for_each with a lambda
    std::for_each(nums.begin(), nums.end(), [&](const auto& elem) {
        std::cout << elem << '\n';
    });
}/*
Key points:
std::for_each iterates over the container range [begin, end).
The lambda [&](const auto& elem) captures the ostream by reference and uses auto for generic element handling.
const auto& avoids unnecessary copies — important for large or non-trivial types.
*/