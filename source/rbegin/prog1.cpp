#include <iostream>
#include <string>

int as_reversed_digits(int n)
{
  std::string t = std::to_string(n);
  std::string r(t.rbegin(), t.rend());
  return std::stoi(r);
}

void show_int_as_reversed_int(int n, std::ostream& out)
{
  out << n << " -> " << as_reversed_digits(n) << std::endl;
}

int as_reversed(int n)
{
  int remainder, reverse = 0;
  while(n > 0)
  {
    remainder = n % 10;
    reverse *= 10;
    reverse += remainder;
    n /= 10;
  }
  return reverse;
}

void show_int_as_reversed_int_numerical(int n, std::ostream& out)
{
  out << n << " -> " << as_reversed(n) << std::endl;
}

void show_int_as_string_and_reversed_1(int n, std::ostream& out)
{
  std::string t = std::to_string(n);
  out << t << std::endl;
  std::string r(t.rbegin(), t.rend());
  out << r << std::endl;
}

void show_int_as_string_and_reversed_2(int n, std::ostream& out)
{
  std::string t = std::to_string(n);
  auto action = std::ostream_iterator<std::string::value_type>(out, "");
  std::copy(t.begin(), t.end(), action);
  out << std::endl;
  std::copy(t.rbegin(), t.rend(), action);
  out << std::endl;
}

int main()
{
  int return_code = 0;

  int n = 12345;
  show_int_as_reversed_int(n, std::cout);
  show_int_as_string_and_reversed_1(n, std::cout);
  show_int_as_string_and_reversed_2(n, std::cout);
  show_int_as_reversed_int_numerical(n, std::cout);

  return return_code;
}