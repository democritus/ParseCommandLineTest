#include <iostream>
#include <wtypes.h>

BOOL CALLBACK CloseWindows(HWND hWnd, long lParam) {
    char buff[255];

    if (IsWindowVisible(hWnd))
    {
        DWORD currentProcId = GetCurrentProcessId();
        DWORD thisProcId;
        GetWindowThreadProcessId(hWnd, &thisProcId);
        if (currentProcId == thisProcId)
        {
            return TRUE;
        }
        GetWindowTextA(hWnd, (LPSTR)buff, 254);
        std::string s(buff);
        if (s.rfind("CppConsole.exe") != std::string::npos
            || s.rfind("ParseCommandLineTest.exe") != std::string::npos)
        {
            try
            {
                PostMessageA(hWnd, WM_CLOSE, 0, 0);
            }
            catch (...)
            {
            }
        }
    }
    return TRUE;
}

int main(int argc, char* argv[])
{
    std::cout << "C++ app test\n\n";
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
    EnumWindows(CloseWindows, 0);
}