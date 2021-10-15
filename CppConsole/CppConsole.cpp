#include <iostream>

int main(int argc, char* argv[])
{
    std::cout << "All args:\n";
    int i;
    for (i = 0; i < argc; ++i)
    {
        std::cout << i << "\t" << argv[i] << "\n";
    }
    std::cout << "\n";
    std::cout << "Press ENTER to exit...\n";
    std::cout << "\n";
    do
    {
    } while (getchar() != '\n');
}