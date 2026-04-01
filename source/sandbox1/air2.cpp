#include <iostream>
#include <vector>
#include <algorithm>

template <typename Container>
void print_all(const Container& c, std::ostream& os = std::cout) {
    std::for_each(c.begin(), c.end(), [&os](const auto& elem) {
        os << elem << '\n';
    });
}

int main() {
    std::vector<int> nums = {10, 20, 30};
    print_all(nums);              // writes to std::cout
    print_all(nums, std::cerr);   // writes to std::cerr
}//Generalizing with a custom ostream: